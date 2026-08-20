using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.API.Authorization
{
    // Gates an endpoint behind specific StaffRole values, read directly from the "staffRole"
    // claim - no bypass. Used for the staff-management screen itself, which must only ever be
    // reachable by StaffRole.Admin, never by anything in the (Admin-managed) StaffPermission
    // table - otherwise a non-admin could be granted the ability to grant themselves admin.
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequireStaffRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly StaffRole[] _allowedRoles;

        public RequireStaffRoleAttribute(params StaffRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var staffRoleClaim = user.FindFirst("staffRole")?.Value;

            if (!int.TryParse(staffRoleClaim, out var staffRole) || !_allowedRoles.Cast<int>().Contains(staffRole))
                context.Result = new ForbidResult();
        }
    }
}
