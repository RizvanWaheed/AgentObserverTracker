using Abp.Authorization;
using AgentObserver.Authorization.Roles;
using AgentObserver.Authorization.Users;

namespace AgentObserver.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
