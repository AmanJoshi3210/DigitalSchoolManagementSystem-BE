using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        // Includes FileStorage + the uploader's User/Student/StaffUser so callers can tell
        // which student or staff member uploaded the document without extra round trips.
        Task<Document?> GetByIdWithDetailsAsync(int id);

        Task<IReadOnlyList<Document>> GetByUploaderAsync(int uploadedByUserId);

        Task<IReadOnlyList<Document>> GetPendingAsync();
    }
}
