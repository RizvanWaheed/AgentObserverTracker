using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using AgentObserver.Authentication.External;
using AgentObserver.Authentication.JwtBearer;
using AgentObserver.Authorization;
using AgentObserver.Authorization.Users;
using AgentObserver.Models.TokenAuth;
using AgentObserver.MultiTenancy;
using RestSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using AgentObserver.Configuration;
using AgentObserver.Users;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Abp.Runtime.Session;

namespace AgentObserver.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : AgentObserverControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly IAbpSession _abpSession;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly UserAppService _userManager;
        private readonly UserManager _abpUserManager;

        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            IAbpSession abpSession,
            UserManager abpUserManager,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager,
            UserAppService userManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _abpUserManager = abpUserManager;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
            _userRegistrationManager = userRegistrationManager;
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        private async Task<UserDetailObjWithInternalDetails> AuthenticateExternalUser(AuthenticateModel model)
        {
            UserDetailObjWithInternalDetails detail = new UserDetailObjWithInternalDetails();
            string remoteUrl = _appConfiguration["Authentication:External:LoginApiUrl"];
            string authKey = _appConfiguration["Authentication:External:AuthKey"];
            var client = new RestClient(remoteUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("password", model.UserNameOrEmailAddress);
            request.AddParameter("username", model.Password);
            request.AddParameter("key", authKey);
            IRestResponse response = await client.ExecuteAsync(request);
            var ExternalDetails = JsonConvert.DeserializeObject<ExternalAuthModel>(response.Content);
            if (ExternalDetails != null && ExternalDetails.Status)
            {
                if (string.IsNullOrEmpty(ExternalDetails.Data.Email))
                {
                    throw new UserFriendlyException(ExternalDetails.Message);
                }
                var registerationModel = new RegisterationViaApiModel()
                {
                    Email = ExternalDetails.Data.Email,
                    FirstName = ExternalDetails.Data.Name,
                    LastName = "",
                    PlainPass = model.Password,
                    UserName = ExternalDetails.Data.Username.ToString()
                };
                detail.UserExternalDetails = ExternalDetails;
                var userStatus = await CreateUserIfNotExsist(registerationModel);
                if (userStatus) detail.UserInternalDetails = registerationModel;

            }
            else throw new UserFriendlyException(ExternalDetails.Message);
            return detail;
        }
        
        private async Task<bool> CreateUserIfNotExsist(RegisterationViaApiModel model)
        {
            var checkUser = await _userManager.GetEntityByUserNameAsync(model.UserName);
            if (checkUser == null) await _userRegistrationManager.RegisterHostUserAsync(model.FirstName,model.LastName,model.Email,model.UserName,model.PlainPass,true);
            return true;
        }
        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var userInternalExternalDetails = await AuthenticateExternalUser(model);
            if (userInternalExternalDetails.UserExternalDetails == null)
            {
                throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                               AbpLoginResultType.InvalidPassword,
                               model.UserNameOrEmailAddress,
                               GetTenancyNameOrNull()
                           );
            }
            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                GetTenancyNameOrNull()
            );
            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id,
                UserExternalAuthToken = userInternalExternalDetails?.UserExternalDetails?.Data?.LogKey
            };
        }
        [HttpGet]
        public async Task<bool> ExternalLogOff(string token)
        {
            var currentUser = await _abpUserManager.GetUserByIdAsync(_abpSession.UserId.Value);
            string remoteUrl = _appConfiguration["Authentication:External:LogoutApiUrl"];
            string authKey = _appConfiguration["Authentication:External:AuthKey"];
            var client = new RestClient(remoteUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cookie", "languageset[rememberlang]=0");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("username", currentUser.UserName);
            request.AddParameter("key", authKey);
            request.AddParameter("log_key", token);
            IRestResponse response = client.Execute(request);
            var responseData = JsonConvert.DeserializeObject<ExternalAuthModel>(response.Content);
            if (!responseData.Status)
            {
                throw new UserFriendlyException(responseData.Message ?? "An error has occurred while processing your request");
            }
            return true;
        }
        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetExternalUserInfo(model);

            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        // Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                Authorization.Users.User.CreateRandomPassword(),
                true
            );

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalUser.Provider,
                    ProviderKey = externalUser.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (userInfo.ProviderKey != model.ProviderKey)
            {
                throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }
    }
}
