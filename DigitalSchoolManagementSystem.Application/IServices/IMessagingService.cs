using DigitalSchoolManagementSystem.Application.DTOs.Messaging;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IMessagingService
    {
        Task<IReadOnlyList<ContactDto>> GetContactsAsync(int currentUserId);
        Task<ConversationDto> StartDirectConversationAsync(int currentUserId, StartDirectConversationDto request);
        Task<ConversationDto> StartQueryConversationAsync(int studentUserId, StartQueryConversationDto request);
        Task<IReadOnlyList<ConversationDto>> GetMyConversationsAsync(int userId);
        Task<ConversationDto> GetConversationAsync(int conversationId, int requestingUserId);
        Task<IReadOnlyList<MessageDto>> GetMessagesAsync(int conversationId, int requestingUserId, int page, int pageSize);
        Task<MessageDto> SendMessageAsync(int conversationId, int senderUserId, SendMessageDto request);
        Task MarkConversationReadAsync(int conversationId, int userId);
        Task<ConversationDto> UpdateStatusAsync(int conversationId, int staffUserId, UpdateConversationStatusDto request);
    }
}
