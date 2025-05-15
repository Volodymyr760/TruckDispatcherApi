using System.Security.Claims;

namespace TruckDispatcherApi.Services
{
    public interface ITokenService
    {
        bool IsTokenExpired(string token);

        string GenerateAccessToken(IEnumerable<Claim> claims, double period);

        string GenerateRandomToken(int length);

        string GetUserEmailFromExpiredToken(string token);
    }
}
