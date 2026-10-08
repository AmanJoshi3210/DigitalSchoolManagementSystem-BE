using DigitalSchoolManagementSystem.Domain.Common;
using System.Collections.Generic;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    public class ReviewStaff : BaseEntity
    {

        // StaffUser.Id is an int in the existing domain model.
        public int StaffUserId { get; set; }

        public decimal AverageReview { get; set; }

        public int TotalReviews { get; set; }

        // Navigation
        public virtual StaffUser Staff { get; set; } = null!;

        public virtual ICollection<ReviewerStudent> Reviews { get; set; }
            = new List<ReviewerStudent>();
    }
}
