using System.ComponentModel.DataAnnotations;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Documents
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }

        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;

        public int UploadedByUserId { get; set; }
        public string UploadedByName { get; set; } = string.Empty;

        // "Student" or "Staff" — resolved from the uploader's Role, so callers can tell
        // which kind of user uploaded the document without a separate lookup.
        public string UploadedByRole { get; set; } = string.Empty;

        public DocumentStatus Status { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByStaffName { get; set; }
        public string? ReviewNotes { get; set; }

        public int? FileStorageId { get; set; }
    }

    public class UploadDocumentRequestDto
    {
        [Required]
        public DocumentType DocumentType { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class ReviewDocumentDto
    {
        [Required]
        public DocumentStatus Status { get; set; }

        [MaxLength(500)]
        public string? ReviewNotes { get; set; }
    }
}
