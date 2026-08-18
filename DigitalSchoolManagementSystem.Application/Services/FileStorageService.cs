using System.Security.Cryptography;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Application.Models;
using DigitalSchoolManagementSystem.Application.Options;
using DigitalSchoolManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private const long DefaultMaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        private readonly IUnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<FileStorageService> _logger;
        private readonly long _maxFileSizeBytes;
        private readonly string _folder;

        public FileStorageService(
            IUnitOfWork unitOfWork,
            Cloudinary cloudinary,
            IOptions<CloudinarySettings> cloudinarySettings,
            IConfiguration configuration,
            ILogger<FileStorageService> logger)
        {
            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
            _logger = logger;

            _maxFileSizeBytes = configuration.GetValue<long?>("FileStorage:MaxFileSizeBytes") ?? DefaultMaxFileSizeBytes;
            _folder = cloudinarySettings.Value.Folder;
        }

        // ---------------------------------------------------------
        // SAVE FILE
        // ---------------------------------------------------------

        public async Task<FileStorage> SaveFileAsync(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(
                    "File cannot be empty.",
                    nameof(file));
            }

            if (file.Length > _maxFileSizeBytes)
            {
                throw new ArgumentException(
                    $"File exceeds the maximum allowed size of {_maxFileSizeBytes} bytes.",
                    nameof(file));
            }

            // Buffer once so the same bytes can be hashed and then handed to Cloudinary —
            // IFormFile's underlying stream isn't guaranteed to support seeking/re-reading.
            var buffer = new MemoryStream();
            await using (var source = file.OpenReadStream())
            {
                await source.CopyToAsync(buffer, cancellationToken);
            }

            buffer.Position = 0;
            var hashBytes = await SHA256.HashDataAsync(buffer, cancellationToken);
            var fileHash = Convert.ToHexString(hashBytes);

            buffer.Position = 0;

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, buffer),
                PublicId = Guid.NewGuid().ToString(),
                Folder = _folder,
                UseFilename = false,
                Overwrite = false
            };

            // "auto" lets Cloudinary route the asset to its image/video/raw pipeline based on
            // content, which is what gives images automatic optimization/transformations.
            var uploadResult = await _cloudinary.UploadAsync(uploadParams, "auto", cancellationToken);

            if (uploadResult.Error != null)
            {
                _logger.LogError(
                    "Cloudinary upload failed for {FileName}: {Error}",
                    file.FileName,
                    uploadResult.Error.Message);

                throw new InvalidOperationException($"File upload failed: {uploadResult.Error.Message}");
            }

            var fileStorage = new FileStorage
            {
                FileName = file.FileName,
                StoredFileName = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString(),
                ContentType = file.ContentType,
                FileSize = file.Length,
                Extension = Path.GetExtension(file.FileName),
                FileHash = fileHash,
                ResourceType = uploadResult.ResourceType
            };

            _logger.LogInformation(
                "Uploaded file {FileName} ({FileSize} bytes) to Cloudinary as {PublicId}",
                fileStorage.FileName,
                fileStorage.FileSize,
                fileStorage.StoredFileName);

            // Save FileStorage record
            await _unitOfWork.FileStorages.AddAsync(fileStorage);
            await _unitOfWork.SaveChangesAsync();

            return fileStorage;
        }

        // ---------------------------------------------------------
        // GET FILE STORAGE DETAILS
        // ---------------------------------------------------------

        public async Task<FileStorage?> GetFileStorageDetailsAsync(
            int fileStorageId)
        {
            return await _unitOfWork.FileStorages
                .GetByIdAsync(fileStorageId);
        }

        // ---------------------------------------------------------
        // GET ACTUAL FILE
        // ---------------------------------------------------------

        public async Task<StoredFileResult?> GetFileAsync(
            int fileStorageId,
            CancellationToken cancellationToken = default)
        {
            var fileStorage =
                await _unitOfWork.FileStorages
                    .GetByIdAsync(fileStorageId);

            if (fileStorage == null)
            {
                return null;
            }

            return new StoredFileResult
            {
                Url = fileStorage.Url,
                FileName = fileStorage.FileName,
                ContentType = fileStorage.ContentType,
                FileSize = fileStorage.FileSize
            };
        }

        // ---------------------------------------------------------
        // DELETE FILE
        // ---------------------------------------------------------

        public async Task<bool> DeleteAsync(
            FileStorage fileStorage)
        {
            if (fileStorage == null)
            {
                return false;
            }

            var resourceType = Enum.Parse<ResourceType>(fileStorage.ResourceType, ignoreCase: true);
            var deletionResult = await _cloudinary.DestroyAsync(new DeletionParams(fileStorage.StoredFileName)
            {
                ResourceType = resourceType
            });

            if (deletionResult.Error != null)
            {
                _logger.LogWarning(
                    "Cloudinary deletion for {PublicId} returned an error: {Error}",
                    fileStorage.StoredFileName,
                    deletionResult.Error.Message);
            }

            // Remove database record
            _unitOfWork.FileStorages.Remove(fileStorage);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
