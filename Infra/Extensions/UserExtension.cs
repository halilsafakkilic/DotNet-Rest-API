using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace netCorePlayground.Infra.Extensions
{
    public static class UserExtension
    {
        public static bool CanView(this IPrincipal user)
        {
            var claims = ((ClaimsIdentity)user.Identity).Claims
                   .Where(c => c.Type.Equals(System.Security.Claims.ClaimTypes.Role))
                   .Select(c => c.Value);

            return claims.Any(x => x.Equals(Policies.ViewRole));
        }
    }
}