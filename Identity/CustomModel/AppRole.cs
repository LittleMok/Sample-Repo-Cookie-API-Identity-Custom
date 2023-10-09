namespace TestIdentity.Identity.CustomModel
{
    public class AppRole
    {
        public static readonly List<AppRole> SeedRoles = new()
        {
            new ()
            {
                Id = 1,
                Name = "Admin",
                Description = "Admin user"
            },
            new ()
            {
                Id = 2,
                Name = "Superuser",
                Description = "Superuser"
            },
            new()
            {
                Id = 3,
                Name = "non-existant",
                Description = "Non existant role for testing purposes"
            },
        };

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public List<AppUser> Users { get; set; }
    }
}
