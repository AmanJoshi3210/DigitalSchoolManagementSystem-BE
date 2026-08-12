using DigitalSchoolManagementSystem.Domain.Common;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // An in-app notification for a single recipient, optionally pointing back at the Conversation that caused it.
    public class Notification : BaseEntity
    {
        public int RecipientUserId { get; set; }
        public User Recipient { get; set; } = null!;

        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        public int? ConversationId { get; set; }
        public Conversation? Conversation { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
