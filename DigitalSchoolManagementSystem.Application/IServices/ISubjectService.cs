using DigitalSchoolManagementSystem.Application.DTOs.Academics;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface ISubjectService
    {
        Task<IReadOnlyList<SubjectDto>> GetAllAsync(string? grade);
        Task<SubjectDto?> GetByIdAsync(int id);
        Task<SubjectDto> CreateAsync(CreateSubjectDto request);
        Task<SubjectDto> UpdateAsync(int id, UpdateSubjectDto request);
        Task DeleteAsync(int id);
    }
}
