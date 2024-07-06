using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AgentObserver.Roles.Dto;
using AgentObserver.Users.Dto;

namespace AgentObserver.UserActivities
{
    public interface IUserActivityAppService : IApplicationService
    {
        Task<UsersActivitiesReturnDto> GetUserStats(DateTime? date, string manager, string department, bool showIndividual);
        Task<ExecutiveStatDto> GetExternalStats(string token);
    }
}
