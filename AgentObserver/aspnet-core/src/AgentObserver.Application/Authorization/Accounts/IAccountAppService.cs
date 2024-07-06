using System.Threading.Tasks;
using Abp.Application.Services;
using AgentObserver.Authorization.Accounts.Dto;

namespace AgentObserver.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
