using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Messaging
{
    public class StartDirectConversationDto
    {
        public int RecipientUserId { get; set; }
        public string InitialMessage { get; set; } = string.Empty;
    }

    public class StartQueryConversationDto
    {
        public int StaffUserId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string InitialMessage { get; set; } = string.Empty;
    }

    public class UpdateConversationStatusDto
    {
        public ConversationStatus Status { get; set; }
    }
}
