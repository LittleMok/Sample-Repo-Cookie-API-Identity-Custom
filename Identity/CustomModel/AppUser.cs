using System.Security.Claims;

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

        public List<Claim> Permissions { get {
                return this.Roles?.SelectMany(x => x.Permissions).Select(x => new Claim("Permission", x.Name)).ToList() ?? new();
            } }
    }
}
