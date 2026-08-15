namespace DigitalSchoolManagementSystem.Application.Models
{
    public class StoredFileResult
    {
        public Stream Stream { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }
    }
}