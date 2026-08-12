using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Messaging
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public ConversationType Type { get; set; }
        public string? Subject { get; set; }
        public ConversationStatus Status { get; set; }

        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public string? LastMessagePreview { get; set; }
        public int UnreadCount { get; set; }

        public IReadOnlyList<ConversationParticipantDto> Participants { get; set; } = new List<ConversationParticipantDto>();
    }

    public class ConversationParticipantDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }

    public class ContactDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
    }
}
