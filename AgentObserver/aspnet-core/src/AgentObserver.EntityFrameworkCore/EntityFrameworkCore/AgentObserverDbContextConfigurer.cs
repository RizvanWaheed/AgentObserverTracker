using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AgentObserver.EntityFrameworkCore
{
    public static class AgentObserverDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AgentObserverDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AgentObserverDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
