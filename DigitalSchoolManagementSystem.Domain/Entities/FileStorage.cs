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

        // Name used to physically store the file
        public string StoredFileName { get; set; } = null!;

        // Physical path / blob path
        public string StoragePath { get; set; } = null!;

        // MIME type: application/pdf, image/png, etc.
        public string ContentType { get; set; } = null!;

        // File size in bytes
        public long FileSize { get; set; }

        // Extension: .pdf, .jpg, .docx
        public string? Extension { get; set; }

        // Optional hash for duplicate detection
        public string? FileHash { get; set; }
    }
}
