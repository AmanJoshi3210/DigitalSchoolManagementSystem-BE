using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Attendance?> GetByStudentAndDateAsync(int studentId, DateTime date) =>
            await DbSet.SingleOrDefaultAsync(a => a.StudentId == studentId && a.Date.Date == date.Date);

        public async Task<IReadOnlyList<Attendance>> GetByStudentAsync(int studentId, DateTime? from, DateTime? to)
        {
            var query = DbSet.Where(a => a.StudentId == studentId);

            if (from.HasValue)
                query = query.Where(a => a.Date.Date >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(a => a.Date.Date <= to.Value.Date);

            return await query.OrderByDescending(a => a.Date).ToListAsync();
        }
    }
}
