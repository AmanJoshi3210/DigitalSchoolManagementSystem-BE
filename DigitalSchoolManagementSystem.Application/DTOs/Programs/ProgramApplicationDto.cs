using System.ComponentModel.DataAnnotations;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Programs
{
    public class ProgramApplicationDto
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentAdmissionNumber { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByStaffName { get; set; }
        public string? ReviewNotes { get; set; }
    }

    public class ReviewApplicationDto
    {
        [Required]
        public ApplicationStatus Status { get; set; }

        [MaxLength(500)]
        public string? ReviewNotes { get; set; }
    }
}
