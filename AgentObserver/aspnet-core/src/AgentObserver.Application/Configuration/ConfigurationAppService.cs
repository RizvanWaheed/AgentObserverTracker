using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using AgentObserver.Configuration.Dto;

namespace AgentObserver.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : AgentObserverAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
