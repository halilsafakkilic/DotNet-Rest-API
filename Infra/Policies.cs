using Microsoft.AspNetCore.Authorization;

namespace netCorePlayground.Infra
{
    public class Policies
    {
        public const string AdminPolicy = "Admin";
        public const string UserPolicy = "User";

        public const string ViewRole = "View";

        public static AuthorizationPolicy AdminPolicyGenerator()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(AdminPolicy).Build();
        }

        public static AuthorizationPolicy ViewRoleGenerator()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(ViewRole).Build();
        }

        public static AuthorizationPolicy UserPolicyGenerator()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(UserPolicy).Build();
        }
    }
}