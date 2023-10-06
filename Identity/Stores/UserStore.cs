using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;
using TestIdentity.Identity.CustomModel;

namespace TestIdentity.Identity.Stores
{
    public class UserStore : IUserStore<AppUser>, IUserRoleStore<AppUser>
    {
        private List<AppUser> _users = new()
        {
            new()
            {
                Id = 1,
                Name = "AdMin",
                Password = "password",
                Email = "admin@example.com",
                Username = "admin",
                Claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Role, "Superuser"),
                    new Claim(ClaimTypes.Role, "Admin")
                }
            }
        };

        public Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            user.Claims ??= new List<Claim>();
            user.Claims.Add(new Claim(ClaimTypes.Role, roleName));

            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            if(_users.Where(x => x.Id == user.Id).Any())
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError()
                {
                    Code = "USER_ALREADY_EXISTS",
                    Description = "The given user already exists"
                }));
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
        }

        public Task<AppUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                var user = _users.FirstOrDefault(x => x.Id.ToString() == userId);

                return Task.FromResult(user);
            }
        }

        public Task<AppUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                var user = _users.FirstOrDefault(x => x.Username.ToLowerInvariant() == normalizedUserName);

                return Task.FromResult(user);
            }
        }

        public Task<string?> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return Task.FromResult(user?.Username.ToLowerInvariant());
            }
        }

        public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            IList<string> result = user.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return Task.FromResult(user?.Id.ToString() ?? "");
            }
        }

        public Task<string?> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return Task.FromResult(user?.Username);
            }
        }

        public Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            IList<AppUser> result = _users.Where(x => x.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == roleName)).ToList();
            return Task.FromResult(result);
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            var result = user.Claims?.Any(x => x.Type == ClaimTypes.Role && x.Value == roleName) ?? false;
            return Task.FromResult(result);
        }

        public Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(AppUser user, string? userName, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(userName, nameof(userName));
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                user.Username = userName;
                return Task.CompletedTask;
            }
        }

        public Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
