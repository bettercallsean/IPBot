using System.IdentityModel.Tokens.Jwt;

namespace IPBot.Infrastructure.Helpers;

public class JwtHelper
{
    public static bool CheckTokenIsValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        
        var tokenTicks = GetTokenExpirationTime(token);
        var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

        var now = DateTime.Now.ToUniversalTime();

        var valid = tokenDate >= now;

        return valid;
    }
    
    private static long GetTokenExpirationTime(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
        var ticks= long.Parse(tokenExp);
        return ticks;
    }
}