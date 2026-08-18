using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Document?> GetByIdWithDetailsAsync(int id) =>
            await Context.Documents
                .Include(d => d.FileStorage)
                .Include(d => d.UploadedByUser)
                    .ThenInclude(u => u.Student)
                .Include(d => d.UploadedByUser)
                    .ThenInclude(u => u.StaffUser)
                .Include(d => d.UploadedByUser)
                    .ThenInclude(u => u.Role)
                .Include(d => d.ReviewedByStaff)
                .SingleOrDefaultAsync(d => d.Id == id);

        public async Task<IReadOnlyList<Document>> GetByUploaderAsync(int uploadedByUserId) =>
            await Context.Documents
                .Include(d => d.FileStorage)
                .Include(d => d.UploadedByUser)
                    .ThenInclude(u => u.Role)
                .Include(d => d.ReviewedByStaff)
                .Where(d => !d.IsDeleted && d.UploadedByUserId == uploadedByUserId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

        public async Task<IReadOnlyList<Document>> GetPendingAsync() =>
            await Context.Documents
                .Include(d => d.FileStorage)
                .Include(d => d.UploadedByUser)
                    .ThenInclude(u => u.Role)
                .Include(d => d.ReviewedByStaff)
                .Where(d => !d.IsDeleted && d.Status == DocumentStatus.Pending)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();
    }
}
