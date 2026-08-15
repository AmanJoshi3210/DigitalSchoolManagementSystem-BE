using DigitalSchoolManagementSystem.Application.Models;
using DigitalSchoolManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IFileStorageService
    {
        Task<FileStorage> SaveFileAsync(
            IFormFile file,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(FileStorage fileStorage);

        Task<FileStorage?> GetFileStorageDetailsAsync(int fileStorageId);

        Task<StoredFileResult?> GetFileAsync(
            int fileStorageId,
            CancellationToken cancellationToken = default);
    }
}
