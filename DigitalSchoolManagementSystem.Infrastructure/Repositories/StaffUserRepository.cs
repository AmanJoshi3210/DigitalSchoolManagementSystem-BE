using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class StaffUserRepository : GenericRepository<StaffUser>, IStaffUserRepository
    {
        public StaffUserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<StaffUser?> GetByUserIdAsync(int userId) =>
            await DbSet.Include(s => s.User).SingleOrDefaultAsync(s => s.UserId == userId);

        public async Task<StaffUser?> GetByEmployeeCodeAsync(string employeeCode) =>
            await DbSet.Include(s => s.User).SingleOrDefaultAsync(s => s.EmployeeCode == employeeCode);
    }
}
