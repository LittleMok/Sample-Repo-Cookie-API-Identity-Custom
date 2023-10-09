using TestIdentity.Identity.CustomModel;

namespace TestIdentity.Identity.DTO
{
    public class RegisterModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public List<int> Roles { get; set; }

        public AppUser AsAppUser()
        {
            return new AppUser()
            {
                Name = Name,
                Email = Email,
                Username = Username,
                Password = Password,
                Roles = this.Roles.Select(x => new AppRole() { Id = x }).ToList()
            };
        }
    }
}
