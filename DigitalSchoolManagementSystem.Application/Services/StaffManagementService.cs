using DigitalSchoolManagementSystem.Application.DTOs.StaffManagement;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Constants;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class StaffManagementService : IStaffManagementService
    {
        private static readonly Dictionary<string, string> PermissionLabels = new()
        {
            [StaffPermissionKeys.ActionHub] = "Action Hub",
            [StaffPermissionKeys.Students] = "Students",
            [StaffPermissionKeys.Programs] = "Programs",
            [StaffPermissionKeys.Messages] = "Messages"
        };

        private readonly IUnitOfWork _unitOfWork;

        public StaffManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<StaffListItemDto>> GetAllStaffAsync()
        {
            var staffUsers = await _unitOfWork.StaffUsers.GetAllWithDetailsAsync();

            return staffUsers.Select(s => new StaffListItemDto
            {
                StaffUserId = s.Id,
                UserId = s.UserId,
                EmployeeCode = s.EmployeeCode,
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
                Email = s.User.Email,
                StaffRole = s.Role,
                Department = s.Department,
                IsActive = s.IsActive,
                Permissions = s.Permissions.Select(p => p.PermissionKey).ToList()
            }).ToList();
        }

        public IReadOnlyList<PermissionCatalogItemDto> GetPermissionCatalog() =>
            StaffPermissionKeys.All
                .Select(key => new PermissionCatalogItemDto { Key = key, Label = PermissionLabels[key] })
                .ToList();

        public async Task<StaffPermissionsDto> GetPermissionsAsync(int staffUserId)
        {
            var staffUser = await _unitOfWork.StaffUsers.GetByIdWithDetailsAsync(staffUserId)
                ?? throw new KeyNotFoundException("Staff user not found.");

            return new StaffPermissionsDto
            {
                StaffUserId = staffUserId,
                Permissions = staffUser.Permissions.Select(p => p.PermissionKey).ToList()
            };
        }

        public async Task<StaffPermissionsDto> SetPermissionsAsync(int staffUserId, IReadOnlyList<string> permissions)
        {
            var staffUser = await _unitOfWork.StaffUsers.GetByIdWithDetailsAsync(staffUserId)
                ?? throw new KeyNotFoundException("Staff user not found.");

            if (staffUser.Role == StaffRole.Admin)
                throw new InvalidOperationException("Cannot set explicit permissions for an Admin; Admins implicitly have all permissions.");

            var invalidKeys = permissions.Where(p => !StaffPermissionKeys.All.Contains(p)).ToList();
            if (invalidKeys.Count > 0)
                throw new ArgumentException($"Unknown permission key(s): {string.Join(", ", invalidKeys)}");

            var requested = permissions.Distinct().ToHashSet();
            var current = staffUser.Permissions.ToList();
            var currentKeys = current.Select(p => p.PermissionKey).ToHashSet();

            var toAdd = requested.Except(currentKeys)
                .Select(key => new StaffPermission { StaffUserId = staffUserId, PermissionKey = key });
            var toRemove = current.Where(p => !requested.Contains(p.PermissionKey));

            await _unitOfWork.StaffPermissions.AddRangeAsync(toAdd);
            _unitOfWork.StaffPermissions.RemoveRange(toRemove);
            await _unitOfWork.SaveChangesAsync();

            return new StaffPermissionsDto { StaffUserId = staffUserId, Permissions = requested.ToList() };
        }
    }
}
