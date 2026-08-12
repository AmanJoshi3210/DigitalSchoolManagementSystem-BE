using DigitalSchoolManagementSystem.Application.DTOs.Messaging;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class MessagingService : IMessagingService
    {
        private const int MaxPreviewLength = 120;

        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public MessagingService(IUnitOfWork unitOfWork, INotificationService notificationService, IRealtimeNotifier realtimeNotifier)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<IReadOnlyList<ContactDto>> GetContactsAsync(int currentUserId)
        {
            var currentUser = await _unitOfWork.Users.GetWithDetailsAsync(currentUserId)
                ?? throw new KeyNotFoundException("User not found.");

            var roleNames = currentUser.Role.Name == "Staff"
                ? new[] { "Staff", "Student" }
                : new[] { "Staff" };

            var users = await _unitOfWork.Users.GetActiveByRoleNamesAsync(roleNames, currentUserId);
            return users.Select(ToContactDto).ToList();
        }

        public async Task<ConversationDto> StartDirectConversationAsync(int currentUserId, StartDirectConversationDto request)
        {
            if (string.IsNullOrWhiteSpace(request.InitialMessage))
                throw new ArgumentException("An initial message is required.");
            if (request.RecipientUserId == currentUserId)
                throw new InvalidOperationException("You cannot start a conversation with yourself.");

            var currentUser = await _unitOfWork.Users.GetWithDetailsAsync(currentUserId)
                ?? throw new KeyNotFoundException("User not found.");
            var recipient = await _unitOfWork.Users.GetWithDetailsAsync(request.RecipientUserId)
                ?? throw new KeyNotFoundException("Recipient not found.");

            if (currentUser.Role.Name != "Staff" && recipient.Role.Name != "Staff")
                throw new InvalidOperationException("Students can only message staff members.");

            var conversation = await _unitOfWork.Conversations.FindDirectConversationAsync(currentUserId, request.RecipientUserId);
            var isNewConversation = conversation is null;

            if (conversation is null)
            {
                conversation = new Conversation
                {
                    Type = ConversationType.Direct,
                    Status = ConversationStatus.Open,
                    CreatedByUserId = currentUserId
                };
                conversation.Participants.Add(new ConversationParticipant { UserId = currentUserId, User = currentUser });
                conversation.Participants.Add(new ConversationParticipant { UserId = request.RecipientUserId, User = recipient });

                await _unitOfWork.Conversations.AddAsync(conversation);
                await _unitOfWork.SaveChangesAsync();
            }

            var (messageDto, _) = await PersistMessageAsync(conversation, currentUserId, request.InitialMessage);

            await _realtimeNotifier.NotifyNewMessageAsync(request.RecipientUserId, messageDto);
            await _notificationService.CreateAsync(
                request.RecipientUserId,
                isNewConversation ? NotificationType.NewConversation : NotificationType.NewMessage,
                isNewConversation ? $"New conversation from {FullName(currentUser)}" : $"New message from {FullName(currentUser)}",
                Preview(request.InitialMessage),
                conversation.Id);

            return await GetConversationAsync(conversation.Id, currentUserId);
        }

        public async Task<ConversationDto> StartQueryConversationAsync(int studentUserId, StartQueryConversationDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Subject))
                throw new ArgumentException("Subject is required.");
            if (string.IsNullOrWhiteSpace(request.InitialMessage))
                throw new ArgumentException("An initial message is required.");

            var student = await _unitOfWork.Users.GetWithDetailsAsync(studentUserId)
                ?? throw new KeyNotFoundException("User not found.");
            if (student.Role.Name != "Student")
                throw new InvalidOperationException("Only students can raise a query.");

            var staff = await _unitOfWork.Users.GetWithDetailsAsync(request.StaffUserId)
                ?? throw new KeyNotFoundException("Staff member not found.");
            if (staff.Role.Name != "Staff")
                throw new InvalidOperationException("Queries can only be raised with a staff member.");

            var conversation = new Conversation
            {
                Type = ConversationType.Query,
                Subject = request.Subject.Trim(),
                Status = ConversationStatus.Open,
                CreatedByUserId = studentUserId
            };
            conversation.Participants.Add(new ConversationParticipant { UserId = studentUserId, User = student });
            conversation.Participants.Add(new ConversationParticipant { UserId = request.StaffUserId, User = staff });

            await _unitOfWork.Conversations.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            var (messageDto, _) = await PersistMessageAsync(conversation, studentUserId, request.InitialMessage);

            await _realtimeNotifier.NotifyNewMessageAsync(request.StaffUserId, messageDto);
            await _notificationService.CreateAsync(
                request.StaffUserId,
                NotificationType.NewConversation,
                $"New query from {FullName(student)}: {conversation.Subject}",
                Preview(request.InitialMessage),
                conversation.Id);

            return await GetConversationAsync(conversation.Id, studentUserId);
        }

        public async Task<IReadOnlyList<ConversationDto>> GetMyConversationsAsync(int userId)
        {
            var conversations = await _unitOfWork.Conversations.GetForUserAsync(userId);

            var dtos = new List<ConversationDto>();
            foreach (var conversation in conversations)
                dtos.Add(await ToConversationDtoAsync(conversation, userId));

            return dtos;
        }

        public async Task<ConversationDto> GetConversationAsync(int conversationId, int requestingUserId)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdWithParticipantsAsync(conversationId)
                ?? throw new KeyNotFoundException("Conversation not found.");

            if (!conversation.Participants.Any(p => p.UserId == requestingUserId && p.IsActive))
                throw new UnauthorizedAccessException("You are not a participant in this conversation.");

            return await ToConversationDtoAsync(conversation, requestingUserId);
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync(int conversationId, int requestingUserId, int page, int pageSize)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdWithParticipantsAsync(conversationId)
                ?? throw new KeyNotFoundException("Conversation not found.");

            if (!conversation.Participants.Any(p => p.UserId == requestingUserId && p.IsActive))
                throw new UnauthorizedAccessException("You are not a participant in this conversation.");

            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 50 : pageSize;

            var messages = await _unitOfWork.Messages.GetPageAsync(conversationId, page, pageSize);
            return messages.Select(m => ToMessageDto(m, m.Sender)).ToList();
        }

        public async Task<MessageDto> SendMessageAsync(int conversationId, int senderUserId, SendMessageDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentException("Message content is required.");

            var conversation = await _unitOfWork.Conversations.GetByIdWithParticipantsAsync(conversationId)
                ?? throw new KeyNotFoundException("Conversation not found.");

            if (conversation.Status == ConversationStatus.Closed)
                throw new InvalidOperationException("This conversation is closed.");

            var (messageDto, updated) = await PersistMessageAsync(conversation, senderUserId, request.Content);

            foreach (var recipient in updated.Participants.Where(p => p.UserId != senderUserId && p.IsActive))
            {
                await _realtimeNotifier.NotifyNewMessageAsync(recipient.UserId, messageDto);
                await _notificationService.CreateAsync(
                    recipient.UserId,
                    NotificationType.NewMessage,
                    $"New message from {messageDto.SenderName}",
                    Preview(messageDto.Content),
                    conversationId);
            }

            return messageDto;
        }

        public async Task MarkConversationReadAsync(int conversationId, int userId)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdWithParticipantsAsync(conversationId)
                ?? throw new KeyNotFoundException("Conversation not found.");

            var participant = conversation.Participants.FirstOrDefault(p => p.UserId == userId && p.IsActive)
                ?? throw new UnauthorizedAccessException("You are not a participant in this conversation.");

            participant.LastReadAt = DateTime.UtcNow;
            _unitOfWork.ConversationParticipants.Update(participant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ConversationDto> UpdateStatusAsync(int conversationId, int staffUserId, UpdateConversationStatusDto request)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdWithParticipantsAsync(conversationId)
                ?? throw new KeyNotFoundException("Conversation not found.");

            if (conversation.Type != ConversationType.Query)
                throw new InvalidOperationException("Only query conversations have a status.");

            var staffParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == staffUserId && p.IsActive)
                ?? throw new UnauthorizedAccessException("You are not a participant in this conversation.");

            if (staffParticipant.User.Role.Name != "Staff")
                throw new UnauthorizedAccessException("Only staff can change a query's status.");

            conversation.Status = request.Status;
            if (request.Status is ConversationStatus.Resolved or ConversationStatus.Closed)
            {
                conversation.ResolvedAt = DateTime.UtcNow;
                conversation.ResolvedByUserId = staffUserId;
            }
            else
            {
                conversation.ResolvedAt = null;
                conversation.ResolvedByUserId = null;
            }

            _unitOfWork.Conversations.Update(conversation);
            await _unitOfWork.SaveChangesAsync();

            if (request.Status is ConversationStatus.Resolved or ConversationStatus.Closed)
            {
                var notificationType = request.Status == ConversationStatus.Resolved
                    ? NotificationType.QueryResolved
                    : NotificationType.QueryClosed;

                foreach (var student in conversation.Participants.Where(p => p.UserId != staffUserId && p.IsActive))
                {
                    var studentDto = await ToConversationDtoAsync(conversation, student.UserId);
                    await _realtimeNotifier.NotifyConversationUpdatedAsync(student.UserId, studentDto);
                    await _notificationService.CreateAsync(
                        student.UserId,
                        notificationType,
                        $"Your query \"{conversation.Subject}\" was marked {request.Status.ToString().ToLowerInvariant()}",
                        "Tap to view the conversation.",
                        conversationId);
                }
            }

            return await ToConversationDtoAsync(conversation, staffUserId);
        }

        private async Task<(MessageDto Dto, Conversation Conversation)> PersistMessageAsync(Conversation conversation, int senderUserId, string content)
        {
            var senderParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == senderUserId && p.IsActive)
                ?? throw new UnauthorizedAccessException("You are not a participant in this conversation.");

            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderUserId = senderUserId,
                Content = content.Trim()
            };
            await _unitOfWork.Messages.AddAsync(message);

            conversation.LastMessageAt = DateTime.UtcNow;
            if (conversation.Status == ConversationStatus.Resolved)
            {
                conversation.Status = ConversationStatus.Open;
                conversation.ResolvedAt = null;
                conversation.ResolvedByUserId = null;
            }
            _unitOfWork.Conversations.Update(conversation);

            await _unitOfWork.SaveChangesAsync();

            return (ToMessageDto(message, senderParticipant.User), conversation);
        }

        private async Task<ConversationDto> ToConversationDtoAsync(Conversation conversation, int requestingUserId)
        {
            var lastMessage = await _unitOfWork.Messages.GetLastMessageAsync(conversation.Id);
            var participant = conversation.Participants.First(p => p.UserId == requestingUserId && p.IsActive);
            var unreadCount = await _unitOfWork.Messages.CountUnreadAsync(conversation.Id, requestingUserId, participant.LastReadAt);

            return new ConversationDto
            {
                Id = conversation.Id,
                Type = conversation.Type,
                Subject = conversation.Subject,
                Status = conversation.Status,
                CreatedByUserId = conversation.CreatedByUserId,
                CreatedAt = conversation.CreatedAt,
                LastMessageAt = conversation.LastMessageAt,
                ResolvedAt = conversation.ResolvedAt,
                LastMessagePreview = lastMessage is null ? null : Preview(lastMessage.Content),
                UnreadCount = unreadCount,
                Participants = conversation.Participants
                    .Where(p => p.IsActive)
                    .Select(p => new ConversationParticipantDto
                    {
                        UserId = p.UserId,
                        FullName = FullName(p.User),
                        Role = p.User.Role.Name,
                        ProfileImageUrl = p.User.ProfileImageUrl
                    })
                    .ToList()
            };
        }

        private static MessageDto ToMessageDto(Message message, User sender) => new()
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderUserId = message.SenderUserId,
            SenderName = FullName(sender),
            SenderRole = sender.Role.Name,
            Content = message.Content,
            IsEdited = message.IsEdited,
            EditedAt = message.EditedAt,
            SentAt = message.CreatedAt
        };

        private static ContactDto ToContactDto(User user) => new()
        {
            UserId = user.Id,
            FullName = FullName(user),
            Role = user.Role.Name,
            Subtitle = user.StaffUser is not null
                ? user.StaffUser.Designation
                : user.Student is not null
                    ? $"Grade {user.Student.Grade} - {user.Student.Section}"
                    : string.Empty
        };

        private static string FullName(User user) => $"{user.FirstName} {user.LastName}".Trim();

        private static string Preview(string content)
        {
            var trimmed = content.Trim();
            return trimmed.Length > MaxPreviewLength ? trimmed[..MaxPreviewLength] + "…" : trimmed;
        }
    }
}
