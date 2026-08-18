using DigitalSchoolManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    public class FileStorage : BaseEntity
    {

        // Original uploaded file name
        public string FileName { get; set; } = null!;

        // Cloudinary public ID used to manage (fetch/delete) the asset
        public string StoredFileName { get; set; } = null!;

        // Cloudinary secure (HTTPS) delivery URL
        public string Url { get; set; } = null!;

        // MIME type: application/pdf, image/png, etc.
        public string ContentType { get; set; } = null!;

        // File size in bytes
        public long FileSize { get; set; }

        // Extension: .pdf, .jpg, .docx
        public string? Extension { get; set; }

        // Optional hash for duplicate detection
        public string? FileHash { get; set; }

        // Cloudinary resource type ("image", "video", or "raw") — required to delete the asset
        public string ResourceType { get; set; } = null!;
    }
}
