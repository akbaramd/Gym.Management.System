namespace GymManagementSystem.Application.Services;

/// <summary>
/// Service for generating and validating JWT tokens.
/// </summary>
public class JwtService
{
    private readonly string _audience;
    private readonly string _issuer;
    private readonly string _secretKey;

    public JwtService(IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        _secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        _issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        _audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured.");
    }

    /// <summary>
    /// Generates an access token with the specified user and session details.
    /// </summary>
    public string GenerateAccessToken(IEnumerable<Claim> claims, DateTime expiredAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiredAt,
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Validates the given JWT token and extracts claims if valid.
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token, out bool isExpired)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero // Disable clock skew for immediate expiration checks
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        isExpired = false;

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }

            throw new SecurityTokenException("Invalid token");
        }
        catch (SecurityTokenExpiredException)
        {
            isExpired = true;
            return null;
        }
        catch
        {
            return null;
        }
    }
}
