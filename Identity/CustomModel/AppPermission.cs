namespace TestIdentity.Identity.CustomModel
{
    public class AppPermission
    {
        public static readonly List<AppPermission> SeedPermissions = new()
        {
            new ()
            {
                Id = 1,
                Name = "ReadSingleForecast",
                RoleId = 1
            },
            new ()
            {
                Id = 2,
                Name = "CreateForecast",
                RoleId = 1
            },
            new()
            {
                Id = 3,
                Name = "non-existant",
                RoleId = 3
            },
        };

        public int Id { get; set; }
        public string Name { get; set; }

        public int RoleId { get; set; }
        public AppRole Role { get; set; }
    }
}
