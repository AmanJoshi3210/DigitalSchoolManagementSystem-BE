using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IStaffUserRepository : IGenericRepository<StaffUser>
    {
        Task<StaffUser?> GetByUserIdAsync(int userId);
        Task<StaffUser?> GetByEmployeeCodeAsync(string employeeCode);
    }
}
