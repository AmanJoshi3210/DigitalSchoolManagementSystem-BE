using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Student?> GetByUserIdAsync(int userId) =>
            await DbSet.Include(s => s.User).SingleOrDefaultAsync(s => s.UserId == userId);

        public async Task<Student?> GetByAdmissionNumberAsync(string admissionNumber) =>
            await DbSet.Include(s => s.User).SingleOrDefaultAsync(s => s.AdmissionNumber == admissionNumber);

        public async Task<Student?> GetByIdWithDetailsAsync(int id) =>
            await DbSet.Include(s => s.User).SingleOrDefaultAsync(s => s.Id == id);

        public async Task<IReadOnlyList<Student>> GetAllWithDetailsAsync() =>
            await DbSet.Include(s => s.User)
                .Where(s => s.IsActive)
                .OrderBy(s => s.Grade).ThenBy(s => s.Section).ThenBy(s => s.RollNumber)
                .ToListAsync();
    }
}
