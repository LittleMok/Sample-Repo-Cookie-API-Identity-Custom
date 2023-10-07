using System.ComponentModel.DataAnnotations;
using TestIdentity.Identity.CustomModel;

namespace TestIdentity.Identity.DTO
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "")]
        public bool? RememberMe { get; set; } = false;

        public AppUser AsAppUser()
        {
            var result = new AppUser()
            {
                Username = this.Username,
                Email = string.Empty,
                Password = string.Empty
            };

            return result;
        }
    }
}
