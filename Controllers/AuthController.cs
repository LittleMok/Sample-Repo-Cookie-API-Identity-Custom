using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            if (this.User?.Identity?.IsAuthenticated ?? false)
            {
                return Ok();
            }

            var user = await _userManager.FindByNameAsync(model.Username ?? "");
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password ?? ""))
            {
                await _signInManager.SignInWithClaimsAsync(user, model.RememberMe ?? false, user.Permissions);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromQuery(Name = "sid")] string sessionId = "")
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                await _signInManager.SignOutAsync();
            }
            else
            {
                var ticketStore = (ITicketStore)_sessionStore;
                await ticketStore.RemoveAsync(sessionId);
            }

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel user)
        {
            var result = await _userManager.CreateAsync(user.AsAppUser(), user.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet("me")]
        public IActionResult GetMyInfo()
        {
            var principal = this.User;
            return Ok(
                    new
                    {
                        Name = principal?.Identity?.Name ?? "Anonimo",
                        Roles = principal?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value),
                        Permissions = principal?.Claims?.Where(x => x.Type == "Permission").Select(x => x.Value),
                        principal?.Identity?.IsAuthenticated,
                        CurrentSid = principal?.Claims?.Where(x => x.Type == "SID").Select(x => x.Value).FirstOrDefault(string.Empty)
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
                    ExpiresUtc = session.Properties.IsPersistent ? null : session.Properties.ExpiresUtc,
                    session.Properties.IsPersistent,
                    session.Properties.AllowRefresh
                };
                result.Add(_new);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var username = User?.Identity?.Name;
            await _sessionStore.RemoveAllAsync(username!);

            return Ok();
        }
    }
}
