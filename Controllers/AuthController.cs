using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TestIdentity.Identity.CustomModel;
using TestIdentity.Identity.DTO;
using TestIdentity.Identity.Stores;

namespace TestIdentity.Controllers
{
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomSessionStore _sessionStore;

        public AuthenticateController(SignInManager<AppUser> signInManager, ITicketStore ticketStore, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _sessionStore = (ICustomSessionStore)ticketStore;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken token)
        {
            var user = await _userManager.FindByNameAsync(model.Username ?? "");
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password ?? ""))
            {
                var customClaims = user.Claims.Where(x => x.Type == "Permission");
                await _signInManager.SignInWithClaimsAsync(user, model.RememberMe ?? false, customClaims);
                return Ok();
            }
            return Unauthorized();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMyInfo()
        {
            var principal = this.User;
            return Ok(
                    new
                    {
                        principal?.Identity?.Name,
                        Roles = principal?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value),
                        Permissions = principal?.Claims?.Where(x => x.Type == "Permission").Select(x => x.Value),
                        principal?.Identity?.IsAuthenticated
                    }
                );
        }

        [Authorize]
        [HttpGet("sessions")]
        public IActionResult GetSessions()
        {
            var username = User.Identity?.Name;   
            var sessions = _sessionStore.GetSessions(username!);
            var result = new List<dynamic>();
            foreach (var session in sessions.Distinct())
            {
                var _new = new
                {
                    SessionId = session.Properties.GetString("SessionId"),
                    session.AuthenticationScheme,
                    session.Properties.IssuedUtc,
                    session.Properties.ExpiresUtc,
                    session.Properties.AllowRefresh
                };
                result.Add(_new);
            }
            return Ok(result);
        }
    }
}
