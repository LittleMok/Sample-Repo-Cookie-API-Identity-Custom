using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestIdentity.Identity.Filters
{
    public class PermissionRequirementFilter : IAuthorizationFilter
    {
        readonly string _permission;

        public PermissionRequirementFilter(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == "Permission" && c.Value == _permission);
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
