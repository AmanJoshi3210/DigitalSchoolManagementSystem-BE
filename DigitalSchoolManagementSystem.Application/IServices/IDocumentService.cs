using DigitalSchoolManagementSystem.Application.DTOs.Documents;
using DigitalSchoolManagementSystem.Application.Models;
using DigitalSchoolManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IDocumentService
    {
        Task<DocumentDto> UploadAsync(
            int uploadedByUserId,
            IFormFile file,
            DocumentType documentType,
            string? description,
            CancellationToken cancellationToken = default);

        Task<DocumentDto?> GetByIdAsync(int documentId);

        Task<IReadOnlyList<DocumentDto>> GetByUploaderAsync(int uploadedByUserId);

        Task<IReadOnlyList<DocumentDto>> GetPendingAsync();

        Task<StoredFileResult?> DownloadAsync(int documentId, CancellationToken cancellationToken = default);

        Task DeleteAsync(int documentId, int requestingUserId, bool requestingUserIsStaff);

        Task<DocumentDto> ReviewAsync(int documentId, int staffUserId, ReviewDocumentDto request);
    }
}
