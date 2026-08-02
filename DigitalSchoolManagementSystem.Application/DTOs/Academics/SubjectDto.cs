namespace DigitalSchoolManagementSystem.Application.DTOs.Academics
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSubjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateSubjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
