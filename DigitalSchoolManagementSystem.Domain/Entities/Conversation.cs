using DigitalSchoolManagementSystem.Domain.Common;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // A 1:1 thread between two users: either a plain Direct chat or a student-raised Query with a resolution lifecycle.
    public class Conversation : BaseEntity
    {
        public ConversationType Type { get; set; }
        public string? Subject { get; set; }
        public ConversationStatus Status { get; set; } = ConversationStatus.Open;

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public DateTime? LastMessageAt { get; set; }

        public DateTime? ResolvedAt { get; set; }
        public int? ResolvedByUserId { get; set; }
        public User? ResolvedByUser { get; set; }

        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
