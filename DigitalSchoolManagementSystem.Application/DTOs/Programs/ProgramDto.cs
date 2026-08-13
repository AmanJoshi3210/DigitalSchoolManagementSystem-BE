using System.ComponentModel.DataAnnotations;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Programs
{
    public class ProgramDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EducationLevel EligibleEducationLevel { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public bool IsActive { get; set; }
        public bool IsAcceptingApplications { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProgramDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public EducationLevel EligibleEducationLevel { get; set; }

        [Required]
        public DateTime ApplicationDeadline { get; set; }
    }

    public class UpdateProgramDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public EducationLevel EligibleEducationLevel { get; set; }

        [Required]
        public DateTime ApplicationDeadline { get; set; }
    }

    public class UpdateProgramStatusDto
    {
        public bool IsActive { get; set; }
    }
}
