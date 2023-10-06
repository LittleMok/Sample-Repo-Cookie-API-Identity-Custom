using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestIdentity.Controllers
{
    [Route("api/test-role-dont-exists")]
    [ApiController]
    [Authorize(Roles = "non-existant")]
    public class AuthorizationByRole : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Test role don't exists works");
        }
    }
}
