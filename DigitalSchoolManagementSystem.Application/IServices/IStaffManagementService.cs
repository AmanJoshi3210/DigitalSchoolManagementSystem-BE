using DigitalSchoolManagementSystem.Application.DTOs.StaffManagement;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IStaffManagementService
    {
        Task<IReadOnlyList<StaffListItemDto>> GetAllStaffAsync();
        IReadOnlyList<PermissionCatalogItemDto> GetPermissionCatalog();
        Task<StaffPermissionsDto> GetPermissionsAsync(int staffUserId);
        Task<StaffPermissionsDto> SetPermissionsAsync(int staffUserId, IReadOnlyList<string> permissions);
    }
}
