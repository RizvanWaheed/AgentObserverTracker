using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace AgentObserver.Localization
{
    public static class AgentObserverLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(AgentObserverConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AgentObserverLocalizationConfigurer).GetAssembly(),
                        "AgentObserver.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
