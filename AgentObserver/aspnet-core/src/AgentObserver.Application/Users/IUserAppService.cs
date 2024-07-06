using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AgentObserver.Authorization.Users;
using AgentObserver.Roles.Dto;
using AgentObserver.Users.Dto;

namespace AgentObserver.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);
        Task<User> GetEntityByUserNameAsync(string username);
    }
}
