using System.IdentityModel.Tokens.Jwt;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using GymManagementSystem.Domain.IdentityContext.DomainService;
using Microsoft.IdentityModel.Tokens;

namespace GymManagementSystem.Application.Services
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly UserDomainService _userDomainService;
        public JwtService(IConfiguration configuration, UserDomainService userDomainService)
        {
            _userDomainService = userDomainService;
            var jwtSettings = configuration.GetSection("JwtSettings");
            _secretKey = jwtSettings["SecretKey"];
            _issuer = jwtSettings["Issuer"];
            _audience = jwtSettings["Audience"];
        }

        public async Task<string> GenerateAccessToken(UserEntity userEntity, DateTime expiredAt)
        {
            var getRolesResult = await _userDomainService.GetRolesAsync(userEntity);
            string[] roles = [];
            if (getRolesResult.IsSuccess)
            {
                roles = getRolesResult.Value.ToArray();
            }
            
            // Define claims for the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userEntity.Id.ToString()),
                new Claim(ClaimTypes.Name, userEntity.PhoneNumber),
                
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); 
            }

            // Generate signing credentials using the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
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

            // Return the serialized token
            return tokenHandler.WriteToken(token);
        }
    }
}
