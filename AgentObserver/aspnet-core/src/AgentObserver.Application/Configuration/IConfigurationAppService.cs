using System.Threading.Tasks;
using AgentObserver.Configuration.Dto;

namespace AgentObserver.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
