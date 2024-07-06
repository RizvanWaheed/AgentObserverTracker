using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace AgentObserver.Controllers
{
    public abstract class AgentObserverControllerBase: AbpController
    {
        protected AgentObserverControllerBase()
        {
            LocalizationSourceName = AgentObserverConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
