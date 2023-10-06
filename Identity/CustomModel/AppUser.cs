using System.Security.Claims;
using System.Security.Principal;

namespace TestIdentity.Identity.CustomModel
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public List<Claim> Claims { get; set; }
    }
}
