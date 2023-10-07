using Microsoft.AspNetCore.Mvc;

namespace TestIdentity.Identity.Filters
{
    public class PermissionRequirementAttribute : TypeFilterAttribute
    {
        public PermissionRequirementAttribute(string Permission) : base(typeof(PermissionRequirementFilter))
        {
            Arguments = new object[] { Permission };
        }
    }
}
