using System.Threading.Tasks;
using Abp.Application.Services;
using AgentObserver.Sessions.Dto;

namespace AgentObserver.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
