using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace netCorePlayground.Infra.Extensions
{
    public static class UserExtension
    {
        public static bool IsTesterUser(this IPrincipal user)
        {
            var claims = ((ClaimsIdentity) user.Identity)?.Claims
                .Where(c => c.Type.Equals(ClaimTypes.Role))
                .Select(c => c.Value);

            return claims != null && claims.Any(x => x.Equals(Policies.TesterUserRole));
        }
    }
}