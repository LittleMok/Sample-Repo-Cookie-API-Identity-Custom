using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestIdentity.Identity.CustomModel;
using TestIdentity.Identity.DTO;
using TestIdentity.JwtAuthorization;

namespace TestIdentity.Controllers
{
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            JwtService jwtService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username ?? "");
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password ?? ""))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _jwtService.GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}
