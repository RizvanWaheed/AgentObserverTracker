using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AgentObserver.Roles.Dto;
using AgentObserver.Users.Dto;
using Microsoft.Extensions.Configuration;

namespace AgentObserver.UserActivities
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
