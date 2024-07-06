using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AgentObserver.Authorization;

namespace AgentObserver
{
    [DependsOn(
        typeof(AgentObserverCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AgentObserverApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AgentObserverAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AgentObserverApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
