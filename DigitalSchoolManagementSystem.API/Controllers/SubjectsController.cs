using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<SubjectDto>>> GetAll([FromQuery] string? grade)
        {
            var subjects = await _subjectService.GetAllAsync(grade);
            return Ok(subjects);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubjectDto>> GetById(int id)
        {
            var subject = await _subjectService.GetByIdAsync(id);
            return subject is null ? NotFound() : Ok(subject);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<SubjectDto>> Create(CreateSubjectDto request)
        {
            try
            {
                var subject = await _subjectService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<SubjectDto>> Update(int id, UpdateSubjectDto request)
        {
            try
            {
                var subject = await _subjectService.UpdateAsync(id, request);
                return Ok(subject);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _subjectService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
