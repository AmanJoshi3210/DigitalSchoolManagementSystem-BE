using System.Security.Claims;
using DigitalSchoolManagementSystem.API.Authorization;
using DigitalSchoolManagementSystem.Application.DTOs.Messaging;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IMessagingService _messagingService;

        public ConversationsController(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        [HttpGet("contacts")]
        public async Task<ActionResult<IReadOnlyList<ContactDto>>> GetContacts()
        {
            var contacts = await _messagingService.GetContactsAsync(CurrentUserId());
            return Ok(contacts);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ConversationDto>>> GetMyConversations()
        {
            var conversations = await _messagingService.GetMyConversationsAsync(CurrentUserId());
            return Ok(conversations);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ConversationDto>> GetById(int id)
        {
            try
            {
                var conversation = await _messagingService.GetConversationAsync(id, CurrentUserId());
                return Ok(conversation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("{id:int}/messages")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessages(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var messages = await _messagingService.GetMessagesAsync(id, CurrentUserId(), page, pageSize);
                return Ok(messages);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost("direct")]
        public async Task<ActionResult<ConversationDto>> StartDirect(StartDirectConversationDto request)
        {
            try
            {
                var conversation = await _messagingService.StartDirectConversationAsync(CurrentUserId(), request);
                return Ok(conversation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("query")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ConversationDto>> StartQuery(StartQueryConversationDto request)
        {
            try
            {
                var conversation = await _messagingService.StartQueryConversationAsync(CurrentUserId(), request);
                return Ok(conversation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/messages")]
        public async Task<ActionResult<MessageDto>> SendMessage(int id, SendMessageDto request)
        {
            try
            {
                var message = await _messagingService.SendMessageAsync(id, CurrentUserId(), request);
                return Ok(message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            try
            {
                await _messagingService.MarkConversationReadAsync(id, CurrentUserId());
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Staff")]
        [RequireStaffPermission(StaffPermissionKeys.Messages)]
        public async Task<ActionResult<ConversationDto>> UpdateStatus(int id, UpdateConversationStatusDto request)
        {
            try
            {
                var conversation = await _messagingService.UpdateStatusAsync(id, CurrentUserId(), request);
                return Ok(conversation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
