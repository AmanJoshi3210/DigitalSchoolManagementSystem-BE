using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.API.Authorization
{
    // Gates a staff endpoint behind one or more grantable page permissions (OR semantics - any
    // one matching claim is enough). Use alongside [Authorize(Roles = "Staff")], not instead of
    // it - this filter only narrows staff access further, it doesn't authenticate on its own.
    // StaffRole.Admin always bypasses, since Admins implicitly have every permission and never
    // get StaffPermission rows.
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequireStaffPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissionKeys;

        public RequireStaffPermissionAttribute(params string[] permissionKeys)
        {
            _permissionKeys = permissionKeys;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                context.Result = new ForbidResult();
                return;
            }

            var staffRoleClaim = user.FindFirst("staffRole")?.Value;
            if (int.TryParse(staffRoleClaim, out var staffRole) && staffRole == (int)StaffRole.Admin)
                return;

            var granted = user.FindAll("permission").Select(c => c.Value);
            if (!granted.Any(_permissionKeys.Contains))
                context.Result = new ForbidResult();
        }
    }
}
