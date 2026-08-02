using System.Security.Claims;
using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.DTOs.Students;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IStudentAcademicsService _studentAcademicsService;

        public StudentsController(IStudentService studentService, IStudentAcademicsService studentAcademicsService)
        {
            _studentService = studentService;
            _studentAcademicsService = studentAcademicsService;
        }

        [HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IReadOnlyList<StudentDto>>> GetAll()
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<StudentDto>> GetMe()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var student = await _studentService.GetByUserIdAsync(userId);
            return student is null ? NotFound() : Ok(student);
        }

        [HttpPut("me")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<StudentDto>> UpdateMe(UpdateOwnProfileDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var student = await _studentService.UpdateOwnProfileAsync(userId, request);
                return Ok(student);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("me/education-status")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<EducationStatusDto>> GetMyEducationStatus()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var status = await _studentService.GetEducationStatusByUserIdAsync(userId);
            return status is null ? NotFound() : Ok(status);
        }

        [HttpGet("me/academics")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<StudentAcademicsSummaryDto>> GetMyAcademics()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var student = await _studentService.GetByUserIdAsync(userId);
            if (student is null)
                return NotFound();

            var summary = await _studentAcademicsService.GetSummaryAsync(student.Id);
            return summary is null ? NotFound() : Ok(summary);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<StudentDto>> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            return student is null ? NotFound() : Ok(student);
        }

        [HttpGet("{id:int}/education-status")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<EducationStatusDto>> GetEducationStatus(int id)
        {
            var status = await _studentService.GetEducationStatusByIdAsync(id);
            return status is null ? NotFound() : Ok(status);
        }

        [HttpGet("{id:int}/academics")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<StudentAcademicsSummaryDto>> GetAcademics(int id)
        {
            var summary = await _studentAcademicsService.GetSummaryAsync(id);
            return summary is null ? NotFound() : Ok(summary);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<StudentDto>> Update(int id, UpdateStudentDto request)
        {
            try
            {
                var student = await _studentService.UpdateAsync(id, request);
                return Ok(student);
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
                await _studentService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
