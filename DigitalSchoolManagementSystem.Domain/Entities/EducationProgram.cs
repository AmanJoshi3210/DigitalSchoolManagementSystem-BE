using DigitalSchoolManagementSystem.Domain.Common;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // A program students can apply to. IsActive (from BaseEntity) is the Active/Inactive toggle - same convention as Subject.
    public class EducationProgram : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EducationLevel EligibleEducationLevel { get; set; }
        public DateTime ApplicationDeadline { get; set; }

        public ICollection<ProgramApplication> Applications { get; set; } = new List<ProgramApplication>();
    }
}
