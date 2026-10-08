using DigitalSchoolManagementSystem.Domain.Common;
using System.Collections.Generic;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    public class ReviewerStudent : BaseEntity
    {
        // ReviewStaff.Id and Student.Id are ints in the existing domain model.
        public int ReviewStaffId { get; set; }

        public int StudentId { get; set; }

        // The admin can create the assignment before the student submits a review.
        public int? ReviewScore { get; set; }

        public string? Comment { get; set; }

        public DateTime? SubmittedAt { get; set; }

        // Navigation
        public virtual ReviewStaff ReviewStaff { get; set; } = null!;

        public virtual Student Student { get; set; } = null!;
    }
}
