using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace ALittleLeaf.Utils
{
    public static class ClaimsExtensions
    {
        public static long GetUserId(this IPrincipal user)
        {
            var claimsPrincipal = (ClaimsPrincipal)user;
            var idClaim = claimsPrincipal.FindFirst("Id");

            if (idClaim != null && long.TryParse(idClaim.Value, out long userId))
            {
                return userId;
            }

            return 0;
        }

        public static string GetUserFullName(this IPrincipal user)
        {
            var claimsPrincipal = (ClaimsPrincipal)user;

            var fullNameClaim = claimsPrincipal.FindFirst("FullName");

            return fullNameClaim?.Value ?? "";
        }

        public static string GetUserEmail(this IPrincipal user)
        {
            var claimsPrincipal = (ClaimsPrincipal)user;

            return claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value
                   ?? claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                   ?? claimsPrincipal.FindFirst("email")?.Value
                   ?? "";
        }

        public static string GetUserRole(this IPrincipal user)
        {
            var claimsPrincipal = (ClaimsPrincipal)user;
            return claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }
    }
}
