using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await DbSet.SingleOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByUsernameAsync(string username) =>
            await DbSet.SingleOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByUsernameOrEmailWithDetailsAsync(string usernameOrEmail) =>
            await DbSet
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.StaffUser)
                .SingleOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

        public async Task<User?> GetWithDetailsAsync(int id) =>
            await DbSet
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.StaffUser)
                .SingleOrDefaultAsync(u => u.Id == id);

        public async Task<bool> EmailExistsAsync(string email) =>
            await DbSet.AnyAsync(u => u.Email == email);

        public async Task<bool> UsernameExistsAsync(string username) =>
            await DbSet.AnyAsync(u => u.Username == username);
    }
}
