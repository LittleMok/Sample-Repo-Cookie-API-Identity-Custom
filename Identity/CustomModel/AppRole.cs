using Microsoft.AspNetCore.Identity;

namespace TestIdentity.Identity.CustomModel
{
    public class AppRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
