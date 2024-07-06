using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AgentObserver.Configuration;

namespace AgentObserver.Web.Host.Startup
{
    [DependsOn(
       typeof(AgentObserverWebCoreModule))]
    public class AgentObserverWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AgentObserverWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AgentObserverWebHostModule).GetAssembly());
        }
    }
}
