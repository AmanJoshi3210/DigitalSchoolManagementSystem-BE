using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // A single chat message within a Conversation. CreatedAt (from BaseEntity) is the sent time;
    // IsActive (from BaseEntity) is used as the soft-delete flag for a retracted message.
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public int SenderUserId { get; set; }
        public User Sender { get; set; } = null!;

        public string Content { get; set; } = string.Empty;

        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
