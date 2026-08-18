namespace DigitalSchoolManagementSystem.Application.Models
{
    public class StoredFileResult
    {
        // CDN delivery URL — callers redirect the client here instead of proxying bytes
        // through this API, so downloads are served directly by Cloudinary.
        public string Url { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }
    }
}