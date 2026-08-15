using System.Security.Cryptography;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Application.Models;
using DigitalSchoolManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private const long DefaultMaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
        private const string DefaultRootPath = "App_Data/Documents";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _rootPath;
        private readonly long _maxFileSizeBytes;

        public FileStorageService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger<FileStorageService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            // Resolve relative to the app's content root so storage works the same on any
            // machine/deployment instead of depending on a hardcoded absolute drive path.
            var configuredPath = configuration["FileStorage:RootPath"] ?? DefaultRootPath;
            _rootPath = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(environment.ContentRootPath, configuredPath);

            _maxFileSizeBytes = configuration.GetValue<long?>("FileStorage:MaxFileSizeBytes") ?? DefaultMaxFileSizeBytes;
        }

        public static string GenerateStoredFileName(string originalFileName)
        {
            string uniqueIdentifier = Guid.NewGuid().ToString();

            string fileExtension =
                Path.GetExtension(originalFileName);

            return $"{uniqueIdentifier}{fileExtension}";
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

            // Make sure the directory exists
            Directory.CreateDirectory(_rootPath);

            // Generate unique stored filename
            string storedFileName =
                GenerateStoredFileName(file.FileName);

            // Full physical path
            string fullPath =
                Path.Combine(_rootPath, storedFileName);

            try
            {
                string fileHash;

                // Save physical file (ReadWrite so we can seek back and hash it below)
                await using (var stream = new FileStream(
                    fullPath,
                    FileMode.Create,
                    FileAccess.ReadWrite,
                    FileShare.None))
                {
                    await file.CopyToAsync(
                        stream,
                        cancellationToken);

                    stream.Position = 0;
                    var hashBytes = await SHA256.HashDataAsync(stream, cancellationToken);
                    fileHash = Convert.ToHexString(hashBytes);
                }

                // Create database entity
                var fileStorage = new FileStorage
                {
                    FileName = file.FileName,
                    StoredFileName = storedFileName,
                    StoragePath = fullPath,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    Extension = Path.GetExtension(file.FileName),
                    FileHash = fileHash
                };

                _logger.LogInformation(
                    "Storing file {FileName} ({FileSize} bytes) as {StoredFileName}",
                    fileStorage.FileName,
                    fileStorage.FileSize,
                    fileStorage.StoredFileName);

                // Save FileStorage record
                await _unitOfWork.FileStorages.AddAsync(fileStorage);
                await _unitOfWork.SaveChangesAsync();

                return fileStorage;
            }
            catch
            {
                // If database operation fails after
                // physical file was created, clean it up.
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                throw;
            }
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

            // Check physical file exists
            if (!File.Exists(fileStorage.StoragePath))
            {
                _logger.LogWarning(
                    "FileStorage {FileStorageId} references a missing physical file at {StoragePath}",
                    fileStorageId,
                    fileStorage.StoragePath);

                return null;
            }

            var stream = new FileStream(
                fileStorage.StoragePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return new StoredFileResult
            {
                Stream = stream,
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

            // Delete physical file if it exists
            if (File.Exists(fileStorage.StoragePath))
            {
                File.Delete(fileStorage.StoragePath);
            }

            // Remove database record
            _unitOfWork.FileStorages.Remove(fileStorage);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
