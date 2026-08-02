using System.Security.Claims;
using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IStudentService _studentService;

        public AttendanceController(IAttendanceService attendanceService, IStudentService studentService)
        {
            _attendanceService = attendanceService;
            _studentService = studentService;
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<AttendanceDto>> Mark(MarkAttendanceDto request)
        {
            try
            {
                var attendance = await _attendanceService.MarkAsync(request);
                return Ok(attendance);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("student/{studentId:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IReadOnlyList<AttendanceDto>>> GetByStudent(
            int studentId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var records = await _attendanceService.GetByStudentAsync(studentId, from, to);
            return Ok(records);
        }

        [HttpGet("student/{studentId:int}/summary")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<AttendanceSummaryDto>> GetStudentSummary(
            int studentId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var summary = await _attendanceService.GetSummaryAsync(studentId, from, to);
            return Ok(summary);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<AttendanceDto>>> GetMine([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (studentId is null)
                return NotFound();

            var records = await _attendanceService.GetByStudentAsync(studentId.Value, from, to);
            return Ok(records);
        }

        [HttpGet("me/summary")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<AttendanceSummaryDto>> GetMySummary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (studentId is null)
                return NotFound();

            var summary = await _attendanceService.GetSummaryAsync(studentId.Value, from, to);
            return Ok(summary);
        }

        private async Task<int?> GetCurrentStudentIdAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var student = await _studentService.GetByUserIdAsync(userId);
            return student?.Id;
        }
    }
}
