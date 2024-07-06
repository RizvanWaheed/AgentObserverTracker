


using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using AgentObserver.Configuration;
using AgentObserver.Controllers;
using AgentObserver.Models.External;
using AgentObserver.Observer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AgentObserver.Web.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ActivityController : AgentObserverControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IRepository<UserActivity, int> _activityRepository;
        private readonly IRepository<UserAvailablitiy, int> _userAvailablitiyRepository;
        private readonly IRepository<UserBreaks, int> _userBreaksRepository;
        private readonly IRepository<ActivityUser, int> _activityUserRepository;
        public ActivityController(IWebHostEnvironment env,
                                    IRepository<UserAvailablitiy, int> userAvailablitiyRepository,
                                    IRepository<UserActivity, int> activityRepository,
                                    IRepository<ActivityUser, int> activityUserRepository,
                                    IRepository<UserBreaks, int> userBreaksRepository)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
            _activityRepository = activityRepository;
            _userAvailablitiyRepository = userAvailablitiyRepository;
            _userBreaksRepository = userBreaksRepository;
            _activityUserRepository = activityUserRepository;
        }

        [HttpGet]
        public async Task<OkObjectResult> GetUserAvailability(string empId, string key)
        {
            if (ValidateAuthKey(key))
            {
                var usernTlogin = GetUserNtLoginById(empId);
                if (string.IsNullOrEmpty(usernTlogin))
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("user not found", System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.NotFound
                    };
                    return Ok(response);
                }
                var record = await _userAvailablitiyRepository
                                .FirstOrDefaultAsync(p => p.UserNTLogin == usernTlogin
                                && p.AvailabilityTime.Day == DateTime.Now.Day
                                && p.AvailabilityTime.Month == DateTime.Now.Month
                                && p.AvailabilityTime.Year == DateTime.Now.Year);
                return Ok(record);
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
        }
        [HttpPost]
        public async Task<OkObjectResult> MarkUserStatus([FromBody] UserStatusInputDto input)
        {
            if (ValidateAuthKey(input.Key))
            {
                try
                {
                    var usernTlogin = GetUserNtLoginById(input.EmpId);
                    if (!string.IsNullOrEmpty(usernTlogin))
                    {
                        var record = await _userAvailablitiyRepository
                                        .FirstOrDefaultAsync(p => p.UserNTLogin == usernTlogin
                                        && p.AvailabilityTime.Day == input.RecordDateTime.Day
                                        && p.AvailabilityTime.Month == input.RecordDateTime.Month
                                        && p.AvailabilityTime.Year == input.RecordDateTime.Year);
                        if (input.InForCheckin)
                        {
                            if (record == null)
                            {
                                await _userAvailablitiyRepository.InsertAsync(new UserAvailablitiy()
                                {
                                    AvailabilityTime = input.RecordDateTime,
                                    CheckinTime = input.RecordDateTime,
                                    IsCheckedIn = true,
                                    IsCheckedOut = false,
                                    UserNTLogin = usernTlogin,
                                    BreakStartTime = null,
                                    BreakEndTime = null,
                                    IsBreak = null
                                });
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("checked-in successfully", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.OK
                                };
                                return Ok(response);
                            }
                            else
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("already checked-in", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.OK
                                };
                                return Ok(response);
                            }
                        }
                        else
                        {
                            if (record == null)
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                                {
                                    Content = new StringContent("User is not checked-in. Please checkin to mark checkout", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.Forbidden
                                };
                                return Ok(response);
                            }
                            if (record != null && record.CheckoutTime == null)
                            {
                                record.IsCheckedOut = true;
                                record.CheckoutTime = input.RecordDateTime;
                                _userAvailablitiyRepository.InsertOrUpdate(record);
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("checked-out successfully", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.OK
                                };
                                return Ok(response);
                            }
                            else
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("already checked-out", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.OK
                                };
                                return Ok(response);
                            }
                        }
                    }
                    else
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("User NtLogin/EmployeeId not found", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.InternalServerError
                        };
                        return Ok(response);
                    }
                }
                catch (Exception ex)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                    return Ok(response);
                }
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
        }
        //public async Task<OkObjectResult> MarkCheckin(string empId, string key)
        //{
        //    if (ValidateAuthKey(key))
        //    {
        //        try
        //        {
        //            var usernTlogin = GetUserNtLoginById(empId);
        //            if (!string.IsNullOrEmpty(usernTlogin))
        //            {
        //                var record = await _userAvailablitiyRepository.FirstOrDefaultAsync(p => p.UserNTLogin == usernTlogin
        //                                && p.AvailabilityTime.Day == DateTime.Now.Day
        //                                && p.AvailabilityTime.Month == DateTime.Now.Month
        //                                && p.AvailabilityTime.Year == DateTime.Now.Year);
        //                if (record == null)
        //                {
        //                    await _userAvailablitiyRepository.InsertAsync(new UserAvailablitiy()
        //                    {
        //                        AvailabilityTime = DateTime.Now,
        //                        CheckinTime = DateTime.Now,
        //                        IsCheckedIn = true,
        //                        IsCheckedOut = false,
        //                        UserNTLogin = usernTlogin,
        //                        BreakStartTime = null,
        //                        BreakEndTime = null,
        //                        IsBreak = null
        //                    });
        //                    //db.SaveChanges();
        //                    var response = new HttpResponseMessage(HttpStatusCode.OK)
        //                    {
        //                        Content = new StringContent("checked-in successfully", System.Text.Encoding.UTF8, "text/plain"),
        //                        StatusCode = HttpStatusCode.OK
        //                    };
        //                    return Ok(response);
        //                }
        //                else
        //                {
        //                    var response = new HttpResponseMessage(HttpStatusCode.OK)
        //                    {
        //                        Content = new StringContent("already checked-in", System.Text.Encoding.UTF8, "text/plain"),
        //                        StatusCode = HttpStatusCode.OK
        //                    };
        //                    return Ok(response);
        //                }
        //            }
        //            else
        //            {
        //                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        //                {
        //                    Content = new StringContent("User not found", System.Text.Encoding.UTF8, "text/plain"),
        //                    StatusCode = HttpStatusCode.NotFound
        //                };
        //                return Ok(response);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        //            {
        //                Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain"),
        //                StatusCode = HttpStatusCode.InternalServerError
        //            };
        //            return Ok(response);
        //        }
        //    }
        //    else
        //    {
        //        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        //        {
        //            Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
        //            StatusCode = HttpStatusCode.Unauthorized
        //        };
        //        return Ok(response);
        //    }
        //}
        private string GetUserNtLoginById(string empId)
        {
            var userNtLogin = _activityUserRepository.FirstOrDefault(p => p.EmployeeId == empId);
            if (userNtLogin != null) return userNtLogin.UserName;
            else return "";
        }
        private bool ValidateAuthKey(string key)
        {
            string authKey = _appConfiguration["Authentication:External:ApiAuthKey"];
            if (!string.IsNullOrEmpty(authKey) && !string.IsNullOrEmpty(key) && authKey == key) return true;
            else return false;
        }
        private async Task<OkObjectResult> MarkCheckout(string empId, string key)
        {
            if (ValidateAuthKey(key))
            {
                try
                {
                    var usernTlogin = GetUserNtLoginById(empId);
                    var record = await _userAvailablitiyRepository.FirstOrDefaultAsync(p => p.UserNTLogin == usernTlogin
                                                                                && p.AvailabilityTime.Day == DateTime.Now.Day
                                                                                && p.AvailabilityTime.Month == DateTime.Now.Month
                                                                                && p.AvailabilityTime.Year == DateTime.Now.Year);
                    if (record.CheckoutTime == null)
                    {
                        record.IsCheckedOut = true;
                        record.CheckoutTime = DateTime.Now;
                        await _userAvailablitiyRepository.InsertOrUpdateAsync(record);
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("checked-out successfully", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.OK
                        };
                        return Ok(response);
                    }
                    else
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("already checked-out", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.OK
                        };
                        return Ok(response);
                    }
                }
                catch (Exception ex)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                    return Ok(response);
                }
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
        }
        [HttpPost]
        public async Task<OkObjectResult> MarkBreak(string empId, string key)
        {
            if (ValidateAuthKey(key))
            {
                var usernTlogin = GetUserNtLoginById(empId);
                var record = await _userAvailablitiyRepository.FirstOrDefaultAsync(p => p.UserNTLogin == usernTlogin
                                                                            && p.AvailabilityTime.Day == DateTime.Now.Day
                                                                            && p.AvailabilityTime.Month == DateTime.Now.Month
                                                                            && p.AvailabilityTime.Year == DateTime.Now.Year);
                if (record != null)
                {
                    var currentTime = DateTime.Now;
                    if (AddBreak(usernTlogin, currentTime))
                    {
                        record.BreakEndTime = null;
                        record.IsBreak = true;
                        record.BreakStartTime = currentTime;
                        await _userAvailablitiyRepository.InsertOrUpdateAsync(record);
                        //db.SaveChanges();
                    }
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("User is not checkedin", System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.Forbidden
                    };
                    return Ok(response);
                }
                var fResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    StatusCode = HttpStatusCode.OK
                };
                return Ok(fResponse);
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }

        }
        [HttpPost]
        public async Task<OkObjectResult> EndBreak(string empId, string key)
        {
            if (ValidateAuthKey(key))
            {
                var usernTlogin = GetUserNtLoginById(empId);
                var record = await _userAvailablitiyRepository.FirstOrDefaultAsync(p => p.UserNTLogin == usernTlogin
                                                                        && p.AvailabilityTime.Day == DateTime.Now.Day
                                                                        && p.AvailabilityTime.Month == DateTime.Now.Month
                                                                        && p.AvailabilityTime.Year == DateTime.Now.Year);
                if (record != null)
                {
                    var userBreak = await _userBreaksRepository
                                    .GetAll()
                                    .OrderByDescending(p => p.CreatedDateTime)
                                    .FirstOrDefaultAsync(p => p.BreakEndTime == null && p.UserNTLogin == usernTlogin);
                    if (userBreak != null)
                    {
                        userBreak.BreakEndTime = DateTime.Now;
                        userBreak.IsBreak = false;
                        _userBreaksRepository.InsertOrUpdate(userBreak);
                        //db.SaveChanges();
                    }
                    else
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                        {
                            Content = new StringContent("User is not on break", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.Forbidden
                        };
                        return Ok(response);
                    }
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("User is not checkedin", System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.Forbidden
                    };
                    return Ok(response);
                }
                var fResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    StatusCode = HttpStatusCode.OK
                };
                return Ok(fResponse);
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
        }
        [HttpPost]
        public bool AddBreak(string ntLogin, DateTime date)
        {
            try
            {
                var UnEndedBreak = _userBreaksRepository
                                    .GetAll()
                                    .OrderByDescending(p => p.CreatedDateTime)
                                    .FirstOrDefault(p => p.UserNTLogin == ntLogin && p.BreakStartTime != null && p.BreakEndTime == null);
                if (UnEndedBreak != null)
                {
                    UnEndedBreak.BreakEndTime = date;
                    UnEndedBreak.IsBreak = false;
                    _userBreaksRepository.InsertOrUpdate(UnEndedBreak);
                }
                _userBreaksRepository.Insert(new UserBreaks()
                {
                    UserNTLogin = ntLogin,
                    BreakEndTime = null,
                    BreakStartTime = date,
                    CreatedDateTime = date,
                    IsBreak = true
                });

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}