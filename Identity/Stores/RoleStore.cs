using Microsoft.AspNetCore.Identity;
using TestIdentity.Identity.CustomModel;

namespace TestIdentity.Identity.Stores
{
    public class RoleStore : IRoleStore<AppRole>
    {
        private readonly DataAccess.AppContext _appContext;

        public RoleStore(DataAccess.AppContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<IdentityResult> CreateAsync(AppRole role, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                _appContext.Roles.Add(role);
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

        public async Task<IdentityResult> DeleteAsync(AppRole role, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                _appContext.Roles.Remove(role);
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

        public async Task<AppRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return await _appContext.Roles.FindAsync(roleId, cancellationToken);
        }

        public async Task<AppRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return _appContext.Roles.First(x => x.Name == normalizedRoleName);
        }

        public Task<string?> GetNormalizedRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return Task.FromResult(role?.Name?.ToLowerInvariant());
            }
        }

        public Task<string> GetRoleIdAsync(AppRole role, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return Task.FromResult(role?.Id.ToString() ?? "");
            }
        }

        public Task<string?> GetRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return Task.FromResult(role?.Name);
            }
        }

        public Task SetNormalizedRoleNameAsync(AppRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(AppRole role, string? roleName, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(roleName, nameof(roleName));
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                role.Name = roleName;
                return Task.CompletedTask;
            }
        }

        public async Task<IdentityResult> UpdateAsync(AppRole role, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                _appContext.Roles.Update(role);
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
