using DigitalSchoolManagementSystem.Application.DTOs.Programs;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class ProgramService : IProgramService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProgramService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ProgramDto>> GetAllAsync(bool includeInactive)
        {
            var programs = includeInactive
                ? await _unitOfWork.Programs.GetAllOrderedAsync()
                : await _unitOfWork.Programs.GetActiveAsync();
            return programs.Select(ToDto).ToList();
        }

        public async Task<ProgramDto?> GetByIdAsync(int id)
        {
            var program = await _unitOfWork.Programs.GetByIdAsync(id);
            return program is null ? null : ToDto(program);
        }

        public async Task<ProgramDto> CreateAsync(CreateProgramDto request)
        {
            var program = new EducationProgram
            {
                Name = request.Name,
                Description = request.Description,
                EligibleEducationLevel = request.EligibleEducationLevel,
                ApplicationDeadline = request.ApplicationDeadline
            };

            await _unitOfWork.Programs.AddAsync(program);
            await _unitOfWork.SaveChangesAsync();
            return ToDto(program);
        }

        public async Task<ProgramDto> UpdateAsync(int id, UpdateProgramDto request)
        {
            var program = await _unitOfWork.Programs.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Program not found.");

            program.Name = request.Name;
            program.Description = request.Description;
            program.EligibleEducationLevel = request.EligibleEducationLevel;
            program.ApplicationDeadline = request.ApplicationDeadline;

            _unitOfWork.Programs.Update(program);
            await _unitOfWork.SaveChangesAsync();
            return ToDto(program);
        }

        public async Task<ProgramDto> UpdateStatusAsync(int id, bool isActive)
        {
            var program = await _unitOfWork.Programs.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Program not found.");

            program.IsActive = isActive;
            _unitOfWork.Programs.Update(program);
            await _unitOfWork.SaveChangesAsync();
            return ToDto(program);
        }

        public async Task<IReadOnlyList<ProgramApplicationDto>> GetApplicationsForProgramAsync(int programId)
        {
            if (!await _unitOfWork.Programs.ExistsAsync(p => p.Id == programId))
                throw new KeyNotFoundException("Program not found.");

            var applications = await _unitOfWork.ProgramApplications.GetByProgramIdAsync(programId);
            return applications.Select(ToApplicationDto).ToList();
        }

        public async Task<ProgramApplicationDto> ApplyAsync(int programId, int studentUserId)
        {
            var student = await _unitOfWork.Students.GetByUserIdAsync(studentUserId)
                ?? throw new KeyNotFoundException("Student not found.");

            var program = await _unitOfWork.Programs.GetByIdAsync(programId)
                ?? throw new KeyNotFoundException("Program not found.");

            if (!program.IsActive)
                throw new InvalidOperationException("This program is not currently active.");

            if (DateTime.UtcNow.Date > program.ApplicationDeadline.Date)
                throw new InvalidOperationException("The application deadline for this program has passed.");

            if (student.EducationLevel is null)
                throw new InvalidOperationException("Please set your education level in your profile before applying.");

            if (student.EducationLevel != program.EligibleEducationLevel)
                throw new InvalidOperationException("You are not eligible for this program.");

            if (await _unitOfWork.ProgramApplications.ExistsForStudentAndProgramAsync(student.Id, programId))
                throw new InvalidOperationException("You have already applied to this program.");

            var application = new ProgramApplication
            {
                StudentId = student.Id,
                ProgramId = programId,
                Status = ApplicationStatus.Pending
            };

            await _unitOfWork.ProgramApplications.AddAsync(application);
            await _unitOfWork.SaveChangesAsync();

            var saved = await _unitOfWork.ProgramApplications.GetByIdWithDetailsAsync(application.Id)
                ?? throw new InvalidOperationException("Failed to load the submitted application.");
            return ToApplicationDto(saved);
        }

        public async Task<IReadOnlyList<ProgramApplicationDto>> GetMyApplicationsAsync(int studentUserId)
        {
            var student = await _unitOfWork.Students.GetByUserIdAsync(studentUserId)
                ?? throw new KeyNotFoundException("Student not found.");

            var applications = await _unitOfWork.ProgramApplications.GetByStudentIdAsync(student.Id);
            return applications.Select(ToApplicationDto).ToList();
        }

        public async Task<ProgramApplicationDto> ReviewApplicationAsync(int applicationId, int staffUserId, ReviewApplicationDto request)
        {
            if (request.Status is not (ApplicationStatus.Approved or ApplicationStatus.Rejected))
                throw new ArgumentException("Status must be Approved or Rejected.");

            var application = await _unitOfWork.ProgramApplications.GetByIdWithDetailsAsync(applicationId)
                ?? throw new KeyNotFoundException("Application not found.");

            if (application.Status != ApplicationStatus.Pending)
                throw new InvalidOperationException("This application has already been reviewed.");

            application.Status = request.Status;
            application.ReviewedAt = DateTime.UtcNow;
            application.ReviewedByStaffId = staffUserId;
            application.ReviewNotes = request.ReviewNotes;

            _unitOfWork.ProgramApplications.Update(application);
            await _unitOfWork.SaveChangesAsync();

            var reviewed = await _unitOfWork.ProgramApplications.GetByIdWithDetailsAsync(application.Id)
                ?? throw new InvalidOperationException("Failed to load the reviewed application.");
            return ToApplicationDto(reviewed);
        }

        private static ProgramDto ToDto(EducationProgram program) => new()
        {
            Id = program.Id,
            Name = program.Name,
            Description = program.Description,
            EligibleEducationLevel = program.EligibleEducationLevel,
            ApplicationDeadline = program.ApplicationDeadline,
            IsActive = program.IsActive,
            IsAcceptingApplications = program.IsActive && DateTime.UtcNow.Date <= program.ApplicationDeadline.Date,
            CreatedAt = program.CreatedAt
        };

        private static ProgramApplicationDto ToApplicationDto(ProgramApplication application) => new()
        {
            Id = application.Id,
            ProgramId = application.ProgramId,
            ProgramName = application.Program.Name,
            StudentId = application.StudentId,
            StudentName = $"{application.Student.User.FirstName} {application.Student.User.LastName}".Trim(),
            StudentAdmissionNumber = application.Student.AdmissionNumber,
            Status = application.Status,
            AppliedAt = application.CreatedAt,
            ReviewedAt = application.ReviewedAt,
            ReviewedByStaffName = application.ReviewedByStaff is null
                ? null
                : $"{application.ReviewedByStaff.FirstName} {application.ReviewedByStaff.LastName}".Trim(),
            ReviewNotes = application.ReviewNotes
        };
    }
}
