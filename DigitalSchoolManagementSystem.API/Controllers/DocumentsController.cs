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
        public async Task<ActionResult<DocumentDto>> Upload(
            [FromForm] UploadDocumentRequestDto request,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            try
            {
                var document = await _documentService.UploadAsync(
                    CurrentUserId(), file, request.DocumentType, request.Description, cancellationToken);

                return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
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

            return File(file.Stream, file.ContentType, file.FileName);
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
