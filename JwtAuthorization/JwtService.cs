using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestIdentity.JwtAuthorization
{
    public class JwtService
    {
        public JwtConfiguration Configuration { get; set; }

        public JwtService(IOptions<JwtConfiguration> config)
        {
            Configuration = config.Value;
        }

        public JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.SigningKey));

            var token = new JwtSecurityToken(
                issuer: Configuration.ValidIssuer,
                audience: Configuration.Audience,
                expires: DateTime.Now.AddMinutes(Configuration.DefaultExpirationMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512)
                );

            return token;
        }
    }
}
