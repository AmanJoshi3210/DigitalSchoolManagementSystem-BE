using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // Master subject taught for a given grade (e.g. Mathematics - Grade 10).
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
