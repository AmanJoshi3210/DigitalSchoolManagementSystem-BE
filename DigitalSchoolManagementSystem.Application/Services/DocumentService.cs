using DigitalSchoolManagementSystem.Application.DTOs.Documents;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Application.Models;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentRepository _documentRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationService _notificationService;

        public DocumentService(
            IUnitOfWork unitOfWork,
            IDocumentRepository documentRepository,
            IFileStorageService fileStorageService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _documentRepository = documentRepository;
            _fileStorageService = fileStorageService;
            _notificationService = notificationService;
        }

        public async Task<DocumentDto> UploadAsync(
            int uploadedByUserId,
            IFormFile file,
            DocumentType documentType,
            string? description,
            CancellationToken cancellationToken = default)
        {
            // FileStorage only ever deals with the physical file — it has no idea a
            // "document" or an "uploader" exists.
            var fileStorage = await _fileStorageService.SaveFileAsync(file, cancellationToken);

            var uploader = await _unitOfWork.Users.GetWithDetailsAsync(uploadedByUserId)
                ?? throw new KeyNotFoundException("Uploading user not found.");

            var document = new Document
            {
                FileStorageId = fileStorage.Id,
                DocumentType = documentType,
                Description = description,
                UploadedByUserId = uploadedByUserId,
                // Staff uploads are self-attested and skip the review queue; student uploads
                // need staff verification via the Action Hub (Document.Status defaults to Pending).
                Status = uploader.Role.Name == "Staff" ? DocumentStatus.Approved : DocumentStatus.Pending
            };

            await _documentRepository.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(document, fileStorage, uploader);
        }

        public async Task<DocumentDto?> GetByIdAsync(int documentId)
        {
            var document = await _documentRepository.GetByIdWithDetailsAsync(documentId);
            if (document is null || document.IsDeleted)
                return null;

            return ToDto(document, document.FileStorage, document.UploadedByUser);
        }

        public async Task<IReadOnlyList<DocumentDto>> GetByUploaderAsync(int uploadedByUserId)
        {
            var documents = await _documentRepository.GetByUploaderAsync(uploadedByUserId);
            return documents.Select(d => ToDto(d, d.FileStorage, d.UploadedByUser)).ToList();
        }

        public async Task<IReadOnlyList<DocumentDto>> GetPendingAsync()
        {
            var documents = await _documentRepository.GetPendingAsync();
            return documents.Select(d => ToDto(d, d.FileStorage, d.UploadedByUser)).ToList();
        }

        public async Task<DocumentDto> ReviewAsync(int documentId, int staffUserId, ReviewDocumentDto request)
        {
            if (request.Status is not (DocumentStatus.Approved or DocumentStatus.Rejected))
                throw new ArgumentException("Status must be Approved or Rejected.");

            var document = await _documentRepository.GetByIdWithDetailsAsync(documentId);
            if (document is null || document.IsDeleted)
                throw new KeyNotFoundException("Document not found.");

            if (document.Status != DocumentStatus.Pending)
                throw new InvalidOperationException("This document has already been reviewed.");

            document.Status = request.Status;
            document.ReviewedAt = DateTime.UtcNow;
            document.ReviewedByStaffId = staffUserId;
            document.ReviewNotes = request.ReviewNotes;

            _documentRepository.Update(document);
            await _unitOfWork.SaveChangesAsync();

            var reviewed = await _documentRepository.GetByIdWithDetailsAsync(documentId)
                ?? throw new InvalidOperationException("Failed to load the reviewed document.");

            await _notificationService.CreateAsync(
                reviewed.UploadedByUserId,
                reviewed.Status == DocumentStatus.Approved ? NotificationType.DocumentApproved : NotificationType.DocumentRejected,
                reviewed.Status == DocumentStatus.Approved ? "Document approved" : "Document rejected",
                reviewed.Status == DocumentStatus.Approved
                    ? $"Your {reviewed.FileStorage.FileName} has been verified."
                    : $"Your {reviewed.FileStorage.FileName} was rejected.{(string.IsNullOrWhiteSpace(reviewed.ReviewNotes) ? "" : $" Reason: {reviewed.ReviewNotes}")}");

            return ToDto(reviewed, reviewed.FileStorage, reviewed.UploadedByUser);
        }

        public async Task<StoredFileResult?> DownloadAsync(int documentId, CancellationToken cancellationToken = default)
        {
            var document = await _documentRepository.GetByIdWithDetailsAsync(documentId);
            if (document is null || document.IsDeleted)
                return null;

            return await _fileStorageService.GetFileAsync(document.FileStorageId, cancellationToken);
        }

        public async Task DeleteAsync(int documentId, int requestingUserId, bool requestingUserIsStaff)
        {
            var document = await _documentRepository.GetByIdWithDetailsAsync(documentId)
                ?? throw new KeyNotFoundException("Document not found.");

            if (!requestingUserIsStaff && document.UploadedByUserId != requestingUserId)
                throw new UnauthorizedAccessException("This document does not belong to you.");

            // Soft delete only: keep the physical file and FileStorage record as an audit trail.
            document.IsDeleted = true;
            _documentRepository.Update(document);
            await _unitOfWork.SaveChangesAsync();
        }

        private static DocumentDto ToDto(Document document, FileStorage fileStorage, User uploader) => new()
        {
            Id = document.Id,
            DocumentType = document.DocumentType,
            Description = document.Description,
            UploadedAt = document.CreatedAt,
            FileName = fileStorage.FileName,
            FileSize = fileStorage.FileSize,
            ContentType = fileStorage.ContentType,
            UploadedByUserId = uploader.Id,
            UploadedByName = $"{uploader.FirstName} {uploader.LastName}",
            UploadedByRole = uploader.Role.Name,
            Status = document.Status,
            ReviewedAt = document.ReviewedAt,
            ReviewedByStaffName = document.ReviewedByStaff is null
                ? null
                : $"{document.ReviewedByStaff.FirstName} {document.ReviewedByStaff.LastName}".Trim(),
            ReviewNotes = document.ReviewNotes
        };
    }
}
