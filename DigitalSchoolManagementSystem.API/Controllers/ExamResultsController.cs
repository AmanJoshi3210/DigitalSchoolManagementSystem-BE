using System.Security.Claims;
using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/exam-results")]
    [Authorize]
    public class ExamResultsController : ControllerBase
    {
        private readonly IExamResultService _examResultService;
        private readonly IStudentService _studentService;

        public ExamResultsController(IExamResultService examResultService, IStudentService studentService)
        {
            _examResultService = examResultService;
            _studentService = studentService;
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ExamResultDto>> Record(RecordExamResultDto request)
        {
            try
            {
                var result = await _examResultService.RecordAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("student/{studentId:int}/exam/{examId:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IReadOnlyList<ExamResultDto>>> GetForStudentExam(int studentId, int examId)
        {
            var results = await _examResultService.GetByStudentAndExamAsync(studentId, examId);
            return Ok(results);
        }

        [HttpGet("student/{studentId:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IReadOnlyList<ExamResultDto>>> GetForStudent(int studentId)
        {
            var results = await _examResultService.GetByStudentAsync(studentId);
            return Ok(results);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ExamResultDto>>> GetMine()
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (studentId is null)
                return NotFound();

            var results = await _examResultService.GetByStudentAsync(studentId.Value);
            return Ok(results);
        }

        [HttpGet("me/exam/{examId:int}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ExamResultDto>>> GetMineForExam(int examId)
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (studentId is null)
                return NotFound();

            var results = await _examResultService.GetByStudentAndExamAsync(studentId.Value, examId);
            return Ok(results);
        }

        private async Task<int?> GetCurrentStudentIdAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var student = await _studentService.GetByUserIdAsync(userId);
            return student?.Id;
        }
    }
}
