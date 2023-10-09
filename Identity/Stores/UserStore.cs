using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using TestIdentity.Identity.CustomModel;

namespace TestIdentity.Identity.Stores
{
    public class UserStore : IUserStore<AppUser>, IUserRoleStore<AppUser>, IUserPasswordStore<AppUser>
    {
        private readonly IRoleStore<AppRole> _roleStore;
        private readonly DataAccess.AppContext _appContext;

        public UserStore(IRoleStore<AppRole> roleStore, DataAccess.AppContext appContext)
        {
            _roleStore = roleStore;
            _appContext = appContext;
        }

        public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            var foundUser = await _appContext.Users.FindAsync(user.Id, cancellationToken);
            var role = await _roleStore.FindByNameAsync(roleName, cancellationToken);
            if(role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            user.Roles.Add(role);
            await _appContext.SaveChangesAsync();
        }

        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                user.Roles = _appContext.Roles.Where(r => user.Roles.Contains(r)).ToList();

                _appContext.Users.Update(user);
                await _appContext.SaveChangesAsync(cancellationToken);
                result = IdentityResult.Success;
            } catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError()
                {
                    Code = ex.Message,
                    Description = ex.StackTrace ?? "Empty description"
                });
            }
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                _appContext.Remove(user);
                await _appContext.SaveChangesAsync(cancellationToken);
                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError()
                {
                    Code = ex.Message,
                    Description = ex.StackTrace ?? "Empty description"
                });
            }
            return result;
        }

        public void Dispose()
        {
        }

        public async Task<AppUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _appContext.Users.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
        }

        public async Task<AppUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = await _appContext.Users
                .Where(x => x.Username.ToLower() == normalizedUserName.ToLower() || x.Email.ToLower() == normalizedUserName.ToLower())
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(cancellationToken);

            return result;
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

        public Task<string?> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Password);
        }

        public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            IList<string> result = user.Roles.Select(x => x.Name).ToList();
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
            IList<AppUser> result = _appContext.Users.Where(x => x.Roles.Any(r => r.Name == roleName)).ToList();
            return Task.FromResult(result);
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            var result = user.Roles.Any(x => x.Name == roleName);
            return Task.FromResult(result);
        }

        public Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            var role = _appContext.Roles.FirstOrDefault(x => x.Name == roleName);
            user.Roles.Remove(role!);
            return _appContext.SaveChangesAsync(cancellationToken);
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.Username = normalizedName.ToLower();
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(AppUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.Password = passwordHash;
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

        public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                var entry = _appContext.Entry(user);
                entry.CurrentValues.SetValues(user);
                await _appContext.SaveChangesAsync(cancellationToken);
                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError()
                {
                    Code = ex.Message,
                    Description = ex.StackTrace ?? "Empty description"
                });
            }
            return result;
        }
    }
}
