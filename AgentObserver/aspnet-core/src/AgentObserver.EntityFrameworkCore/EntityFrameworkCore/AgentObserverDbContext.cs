using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using AgentObserver.Authorization.Roles;
using AgentObserver.Authorization.Users;
using AgentObserver.MultiTenancy;
using AgentObserver.Observer;

namespace AgentObserver.EntityFrameworkCore
{
    public class AgentObserverDbContext : AbpZeroDbContext<Tenant, Role, User, AgentObserverDbContext>
    {
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserAvailablitiy> UserAvailablitiy { get; set; }
        public DbSet<ActivityUser> ActivityUser { get; set; }
        public DbSet<UserBreaks> UserBreaks { get; set; }

        public AgentObserverDbContext(DbContextOptions<AgentObserverDbContext> options)
            : base(options)
        {
        }
    }
}
