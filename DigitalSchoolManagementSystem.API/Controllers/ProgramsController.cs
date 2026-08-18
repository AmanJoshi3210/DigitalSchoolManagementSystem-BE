using System.Security.Claims;
using DigitalSchoolManagementSystem.Application.DTOs.Programs;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/programs")]
    [Authorize]
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _programService;

        public ProgramsController(IProgramService programService)
        {
            _programService = programService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProgramDto>>> GetAll()
        {
            var programs = await _programService.GetAllAsync(includeInactive: User.IsInRole("Staff"));
            return Ok(programs);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProgramDto>> GetById(int id)
        {
            var program = await _programService.GetByIdAsync(id);
            return program is null ? NotFound() : Ok(program);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ProgramDto>> Create(CreateProgramDto request)
        {
            var program = await _programService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = program.Id }, program);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ProgramDto>> Update(int id, UpdateProgramDto request)
        {
            try
            {
                return Ok(await _programService.UpdateAsync(id, request));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ProgramDto>> UpdateStatus(int id, UpdateProgramStatusDto request)
        {
            try
            {
                return Ok(await _programService.UpdateStatusAsync(id, request.IsActive));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id:int}/applications")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IReadOnlyList<ProgramApplicationDto>>> GetApplications(int id)
        {
            try
            {
                return Ok(await _programService.GetApplicationsForProgramAsync(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("applications/pending")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IReadOnlyList<ProgramApplicationDto>>> GetPendingApplications()
        {
            return Ok(await _programService.GetPendingApplicationsAsync());
        }

        [HttpPost("{id:int}/apply")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ProgramApplicationDto>> Apply(int id)
        {
            try
            {
                return Ok(await _programService.ApplyAsync(id, CurrentUserId()));
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

        [HttpGet("me/applications")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ProgramApplicationDto>>> GetMyApplications()
        {
            try
            {
                return Ok(await _programService.GetMyApplicationsAsync(CurrentUserId()));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("applications/{applicationId:int}/review")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ProgramApplicationDto>> ReviewApplication(int applicationId, ReviewApplicationDto request)
        {
            try
            {
                return Ok(await _programService.ReviewApplicationAsync(applicationId, CurrentUserId(), request));
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

        private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
