using DigitalSchoolManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    public class Document :BaseEntity
    {

        // Reference to the physical stored file
        public int FileStorageId { get; set; }

        public FileStorage FileStorage { get; set; } = null!;

        // Business information
        public DocumentType DocumentType { get; set; }

        public string? Description { get; set; }

        public bool IsDeleted { get; set; }

        // Who uploaded this document (student or staff — resolve via UploadedByUser.Student / .StaffUser)
        public int UploadedByUserId { get; set; }

        public User UploadedByUser { get; set; } = null!;
    }

    public enum DocumentType
    {
        Passport = 1,
        OfferLetter = 2,
        Certificate = 3,
        ProfilePhoto = 4,
        Other = 5
    }
}


