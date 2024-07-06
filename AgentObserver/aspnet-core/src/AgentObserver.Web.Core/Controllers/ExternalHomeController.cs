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

using AgentObserver.Users;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using AgentObserver.Users.Dto;
using AgentObserver.Configuration;

namespace AgentObserver.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ExternalHomeController : AgentObserverControllerBase
    {
        private readonly IConfigurationRoot _appConfiguration;
     
        public ExternalHomeController(IWebHostEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }


        //[HttpGet]
        //public async Task<ExecutiveStatsDto> GetExectiveStats(string token)
        //{
        //    string remoteUrl = _appConfiguration["Authentication:External:StatsApiUrl"];
        //    string authKey = _appConfiguration["Authentication:External:AuthKey"];
        //    var client = new RestClient(remoteUrl);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AlwaysMultipartFormData = true;
        //    request.AddParameter("key", authKey);
        //    request.AddParameter("log_key", token ?? "TWpJPQ==");
        //    IRestResponse response = await client.ExecuteAsync(request);
        //    var result = JsonConvert.DeserializeObject<ExecutiveStats>(response.Content);
        //    if (!result.Status)
        //    {
        //        throw new UserFriendlyException(L("UnAuthorizedForStats"));
        //    }
        //    return new ExecutiveStatsDto() 
        //    {
        //        AbsentDays = result.Data.AbsentDays,
        //        LeaveDays = result.Data.LeaveDays,
        //        AbsentLeaveDays = result.Data.AbsentLeaveDays,
        //        UserAbsentDays = result.Data.UserAbsentDays,
        //        UserAbsentLeaveDays = result.Data.UserAbsentLeaveDays,
        //        AbsentLeavePerc = result.Data.AbsentLeavePerc,
        //        AbsentPerc = result.Data.AbsentPerc,
        //        AbsentUsers = result.Data.AbsentUsers,
        //        AutoLogoutPerDay = result.Data.AutoLogoutPerDay,
        //        EarlyLeavers = result.Data.EarlyLeavers,
        //        EarlyLeaversPerc = result.Data.EarlyLeaversPerc,
        //        ExtraHours = result.Data.ExtraHours,
        //        ExtraTime = result.Data.ExtraTime,
        //        IntimePerc = result.Data.IntimePerc,
        //        InTimeUsers = result.Data.InTimeUsers,
        //        LateLeavers = result.Data.LateLeavers,
        //        Message = result.Message,
        //        MonthToDateAbsentPerc = result.Data.MonthToDateAbsentPerc,
        //        NonWorkingHours = result.Data.NonWorkingHours,
        //        NonWorkingTime = result.Data.NonWorkingTime,
        //        OnFloorUsers = result.Data.OnFloorUsers,
        //        OooHours = result.Data.OooHours,
        //        OutOfOffice = result.Data.OutOfOffice,
        //        OutOfOfficeHour = result.Data.OutOfOfficeHour,
        //        OutOfOfficeWithAtt = result.Data.OutOfOfficeWithAtt,
        //        OutOfOfficeWithoutAtt = result.Data.OutOfOfficeWithoutAtt,
        //        PresentDays = result.Data.PresentDays,
        //        PresentLeaveScheduledUsers = result.Data.PresentLeaveScheduledUsers,
        //        PresentNonScheduledUsers = result.Data.PresentNonScheduledUsers,
        //        PresentOffsiteScheduledUsers = result.Data.PresentOffsiteScheduledUsers,
        //        UserRemainingLeaveDays = result.Data.UserRemainingLeaveDays,
        //        SpentHours = result.Data.SpentHours,
        //        PresentUsers = result.Data.PresentUsers,
        //        WorkingHours = result.Data.WorkingHours,
        //        WorkingTime = result.Data.WorkingTime
        //    };
        //}

    }
}
