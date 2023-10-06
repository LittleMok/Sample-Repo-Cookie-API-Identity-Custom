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

        public override async Task<bool> CheckPasswordAsync(AppUser user, string password)
        {
            CancellationTokenSource source = new();
            CancellationToken token = source.Token;

            var id = await Store.GetUserIdAsync(user, token);
            var found = await Store.FindByIdAsync(id, token);

            if(found != null && found.Password == password)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public override async Task<AppUser?> FindByNameAsync(string userName)
        {
            CancellationTokenSource source = new();
            CancellationToken token = source.Token;

            var normalized = await Store.GetNormalizedUserNameAsync(new AppUser()
            {
                Username = userName,
            }, token);
            var found = await Store.FindByNameAsync(normalized ?? "", token);
            return found;
        }
    }
}
