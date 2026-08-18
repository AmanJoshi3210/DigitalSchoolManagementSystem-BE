using System.Security.Claims;
using DigitalSchoolManagementSystem.Application.DTOs.Documents;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<DocumentDto>> Upload(
            [FromForm] UploadDocumentRequestDto request,
            [FromForm] IFormFile file,
            CancellationToken cancellationToken)
        {
            try
            {
                var document = await _documentService.UploadAsync(
                    CurrentUserId(),
                    file,
                    request.DocumentType,
                    request.Description,
                    cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = document.Id },
                    document);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DocumentDto>> GetById(int id)
        {
            var document = await _documentService.GetByIdAsync(id);
            if (document is null)
                return NotFound();

            if (!User.IsInRole("Staff") && document.UploadedByUserId != CurrentUserId())
                return Forbid();

            return Ok(document);
        }

        [HttpGet("me")]
        public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetMine()
        {
            var documents = await _documentService.GetByUploaderAsync(CurrentUserId());
            return Ok(documents);
        }

        [HttpGet("user/{userId:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetByUser(int userId)
        {
            var documents = await _documentService.GetByUploaderAsync(userId);
            return Ok(documents);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetPending()
        {
            var documents = await _documentService.GetPendingAsync();
            return Ok(documents);
        }

        [HttpPut("{id:int}/review")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<DocumentDto>> Review(int id, ReviewDocumentDto request)
        {
            try
            {
                return Ok(await _documentService.ReviewAsync(id, CurrentUserId(), request));
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

        [HttpGet("{id:int}/download")]
        public async Task<IActionResult> Download(int id, CancellationToken cancellationToken)
        {
            var document = await _documentService.GetByIdAsync(id);
            if (document is null)
                return NotFound();

            if (!User.IsInRole("Staff") && document.UploadedByUserId != CurrentUserId())
                return Forbid();

            var file = await _documentService.DownloadAsync(id, cancellationToken);
            if (file is null)
                return NotFound();

            // Authorization already checked above; redirect to Cloudinary's CDN so the file is
            // streamed directly to the client instead of proxied through this API.
            return Redirect(file.Url);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _documentService.DeleteAsync(id, CurrentUserId(), User.IsInRole("Staff"));
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

        private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
