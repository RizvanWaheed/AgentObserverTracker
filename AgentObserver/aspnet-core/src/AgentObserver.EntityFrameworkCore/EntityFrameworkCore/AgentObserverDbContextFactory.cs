using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using AgentObserver.Configuration;
using AgentObserver.Web;

namespace AgentObserver.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AgentObserverDbContextFactory : IDesignTimeDbContextFactory<AgentObserverDbContext>
    {
        public AgentObserverDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AgentObserverDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AgentObserverDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AgentObserverConsts.ConnectionStringName));

            return new AgentObserverDbContext(builder.Options);
        }
    }
}
