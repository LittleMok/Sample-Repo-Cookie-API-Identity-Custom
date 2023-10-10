using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestIdentity.Identity.Filters
{
    public class PermissionRequirementFilter : IAuthorizationFilter
    {
        readonly string[] _permissions;

        public PermissionRequirementFilter(params string[] permissions)
        {
            _permissions = permissions;
        }

        public PermissionRequirementFilter(string permissions)
        {
            _permissions = permissions.Split(",");
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == "Permission" && _permissions.Contains(c.Value));
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
