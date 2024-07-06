using Abp.Application.Services;
using AgentObserver.MultiTenancy.Dto;

namespace AgentObserver.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

