using System.Security.Claims;
using AspDotnetBoilerplate.src.Domain;

namespace AspDotnetBoilerplate.src.Shared.Utils;


public interface ITokenService<TKey> where TKey : IEquatable<TKey>
{
    string GenerateAccessToken(User user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    List<Claim> GetClaimsFromToken(string token);
}