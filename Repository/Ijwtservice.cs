using System.Security.Claims;
using CineMatrix_API.Models;

namespace CineMatrix_API.Repository
{
    public interface Ijwtservice
    {
        string GenerateJwtToken(User user);
        string? GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
