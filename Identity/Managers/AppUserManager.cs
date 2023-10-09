using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TestIdentity.Identity.CustomModel;

namespace TestIdentity.Identity.Managers
{
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AppUser> passwordHasher, IEnumerable<IUserValidator<AppUser>> userValidators, IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public override Task<bool> CheckPasswordAsync(AppUser user, string password)
        {
            return base.CheckPasswordAsync(user, password);
        }

        public override async Task<AppUser?> FindByNameAsync(string userName)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            var cancellationToken = source.Token;

            return await Store.FindByNameAsync(userName, cancellationToken);
        }

        public override Task<IdentityResult> CreateAsync(AppUser user, string password)
        {
            return base.CreateAsync(user, password);
        }
    }
}
