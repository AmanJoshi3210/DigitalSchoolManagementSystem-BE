using DigitalSchoolManagementSystem.API.Authorization;
using DigitalSchoolManagementSystem.Application.DTOs.StaffManagement;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    // Admin-only: managing which staff users can see which staff pages. Gated by StaffRole
    // directly (RequireStaffRoleAttribute), never by a grantable permission - otherwise a
    // non-admin could be granted the ability to grant themselves admin.
    [ApiController]
    [Route("api/staff-management")]
    [Authorize(Roles = "Staff")]
    [RequireStaffRole(StaffRole.Admin)]
    public class StaffManagementController : ControllerBase
    {
        private readonly IStaffManagementService _staffManagementService;

        public StaffManagementController(IStaffManagementService staffManagementService)
        {
            _staffManagementService = staffManagementService;
        }

        [HttpGet("staff")]
        public async Task<ActionResult<IReadOnlyList<StaffListItemDto>>> GetAllStaff()
        {
            var staff = await _staffManagementService.GetAllStaffAsync();
            return Ok(staff);
        }

        [HttpGet("permissions/catalog")]
        public ActionResult<IReadOnlyList<PermissionCatalogItemDto>> GetPermissionCatalog()
        {
            return Ok(_staffManagementService.GetPermissionCatalog());
        }

        [HttpGet("staff/{staffUserId:int}/permissions")]
        public async Task<ActionResult<StaffPermissionsDto>> GetPermissions(int staffUserId)
        {
            try
            {
                return Ok(await _staffManagementService.GetPermissionsAsync(staffUserId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("staff/{staffUserId:int}/permissions")]
        public async Task<ActionResult<StaffPermissionsDto>> SetPermissions(int staffUserId, SetStaffPermissionsDto request)
        {
            try
            {
                return Ok(await _staffManagementService.SetPermissionsAsync(staffUserId, request.Permissions));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
