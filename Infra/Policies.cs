using Microsoft.AspNetCore.Authorization;

namespace netCorePlayground.Infra
{
    public class Policies
    {
        public const string UserPolicy = "User";

        public const string AdminRole = "Admin";
        public const string TesterUserRole = "BetaUser";

        public static AuthorizationPolicy UserPolicyGenerator()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(UserPolicy).Build();
        }

        public static AuthorizationPolicy AdminRoleGenerator()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(AdminRole).Build();
        }

        public static AuthorizationPolicy TesterUserRoleGenerator()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(TesterUserRole).Build();
        }
    }
}