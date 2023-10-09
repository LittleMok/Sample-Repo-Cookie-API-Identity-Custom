using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Security.Principal;

namespace TestIdentity.Identity.CustomModel
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public List<AppRole> Roles { get; set; }
    }
}
