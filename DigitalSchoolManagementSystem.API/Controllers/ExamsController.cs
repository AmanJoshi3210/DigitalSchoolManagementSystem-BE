using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/exams")]
    [Authorize]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ExamDto>>> GetAll([FromQuery] string? grade, [FromQuery] string? academicYear)
        {
            var exams = await _examService.GetAllAsync(grade, academicYear);
            return Ok(exams);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExamDto>> GetById(int id)
        {
            var exam = await _examService.GetByIdAsync(id);
            return exam is null ? NotFound() : Ok(exam);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ExamDto>> Create(CreateExamDto request)
        {
            var exam = await _examService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = exam.Id }, exam);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ExamDto>> Update(int id, UpdateExamDto request)
        {
            try
            {
                var exam = await _examService.UpdateAsync(id, request);
                return Ok(exam);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _examService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{examId:int}/subjects")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ExamSubjectDto>> AddSubject(int examId, AddExamSubjectDto request)
        {
            try
            {
                var examSubject = await _examService.AddSubjectAsync(examId, request);
                return CreatedAtAction(nameof(GetById), new { id = examId }, examSubject);
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

        [HttpPut("{examId:int}/subjects/{examSubjectId:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ExamSubjectDto>> UpdateSubject(int examId, int examSubjectId, UpdateExamSubjectDto request)
        {
            try
            {
                var examSubject = await _examService.UpdateSubjectAsync(examId, examSubjectId, request);
                return Ok(examSubject);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{examId:int}/subjects/{examSubjectId:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RemoveSubject(int examId, int examSubjectId)
        {
            try
            {
                await _examService.RemoveSubjectAsync(examId, examSubjectId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
