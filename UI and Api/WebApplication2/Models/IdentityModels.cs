using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace AgentObserverWeb.Models
{
    //// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{

    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
    //        // Add custom user claims here
    //        return userIdentity;
    //    }
    //}

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserAvailablitiy> UserAvailablitiy { get; set; }
        public DbSet<ActivityUser> ActivityUser { get; set; }
        public DbSet<UserBreaks> UserBreaks { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}