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
            var roleEntity = modelBuilder.Entity<AppRole>();
            roleEntity.HasKey(x => x.Id);
            roleEntity.HasData(AppRole.SeedRoles);

            var userEntity = modelBuilder.Entity<AppUser>();
            userEntity.HasKey(x => x.Id);
            userEntity.HasMany(x => x.Roles).WithMany(x => x.Users);
        }
    }

    public static class AppContextInstaller
    {
        public static IServiceCollection AddInMemoryDatabase(this IServiceCollection services)
        {
            services.AddDbContext<AppContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            return services;
        }
    }
}
