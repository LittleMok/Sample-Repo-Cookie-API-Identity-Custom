using Microsoft.AspNetCore.Identity;
using TestIdentity.Identity.CustomModel;

namespace TestIdentity.Identity.Stores
{
    public class RoleStore : IRoleStore<AppRole>
    {
        private List<AppRole> roles = new()
        {
            new()
            {
                Id = 1,
                Name = "Superuser",
                Description = "User with all access",
            }
        };

        public Task<IdentityResult> CreateAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
        }

        public Task<AppRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            while(true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                var role = roles.Where(x => x.Id.ToString() == roleId).FirstOrDefault();
                
                return Task.FromResult(role);
            }
        }

        public Task<AppRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                var role = roles.Where(x => x.Name.ToLowerInvariant() == normalizedRoleName).FirstOrDefault();

                return Task.FromResult(role);
            }
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

        public Task<IdentityResult> UpdateAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
