using Microsoft.EntityFrameworkCore;
using TestIdentity.Identity.CustomModel;

namespace TestIdentity.DataAccess
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options) { }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppRole> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var permissionEntity = modelBuilder.Entity<AppPermission>();
            permissionEntity.HasKey(x => x.Id);
            permissionEntity.HasOne(x => x.Role)
                .WithMany(x => x.Permissions)
                .HasForeignKey(x => x.RoleId);
            
            var roleEntity = modelBuilder.Entity<AppRole>();
            roleEntity.HasKey(x => x.Id);
            roleEntity.HasMany(x => x.Permissions)
                .WithOne(x => x.Role);

            var userEntity = modelBuilder.Entity<AppUser>();
            userEntity.HasKey(x => x.Id);
            userEntity.HasMany(x => x.Roles).WithMany(x => x.Users);
            userEntity.Ignore(x => x.Permissions);

            roleEntity.HasData(AppRole.SeedRoles);
            permissionEntity.HasData(AppPermission.SeedPermissions);
        }
    }

    public static class AppContextInstaller
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfigurationRoot configuration, string connectionStringName)
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);

            services.AddDbContext<AppContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            return services;
        }
    }
}
