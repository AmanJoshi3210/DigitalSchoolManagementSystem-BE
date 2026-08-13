using DigitalSchoolManagementSystem.Application.DTOs.Programs;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IProgramService
    {
        Task<IReadOnlyList<ProgramDto>> GetAllAsync(bool includeInactive);
        Task<ProgramDto?> GetByIdAsync(int id);
        Task<ProgramDto> CreateAsync(CreateProgramDto request);
        Task<ProgramDto> UpdateAsync(int id, UpdateProgramDto request);
        Task<ProgramDto> UpdateStatusAsync(int id, bool isActive);
        Task<IReadOnlyList<ProgramApplicationDto>> GetApplicationsForProgramAsync(int programId);
        Task<ProgramApplicationDto> ApplyAsync(int programId, int studentUserId);
        Task<IReadOnlyList<ProgramApplicationDto>> GetMyApplicationsAsync(int studentUserId);
        Task<ProgramApplicationDto> ReviewApplicationAsync(int applicationId, int staffUserId, ReviewApplicationDto request);
    }
}
