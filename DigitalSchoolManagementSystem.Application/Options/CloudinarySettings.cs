namespace DigitalSchoolManagementSystem.Application.Options
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = null!;

        public string ApiKey { get; set; } = null!;

        public string ApiSecret { get; set; } = null!;

        // Base folder under which all uploads for this app are organized in the Cloudinary media library.
        public string Folder { get; set; } = "DigitalSchoolManagementSystem/documents";
    }
}
