using System.Security.Claims;
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

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
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

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<StudentDto>> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            return student is null ? NotFound() : Ok(student);
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
