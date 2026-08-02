using DigitalSchoolManagementSystem.Application.DTOs.Academics;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IExamService
    {
        Task<IReadOnlyList<ExamDto>> GetAllAsync(string? grade, string? academicYear);
        Task<ExamDto?> GetByIdAsync(int id);
        Task<ExamDto> CreateAsync(CreateExamDto request);
        Task<ExamDto> UpdateAsync(int id, UpdateExamDto request);
        Task DeleteAsync(int id);
        Task<ExamSubjectDto> AddSubjectAsync(int examId, AddExamSubjectDto request);
        Task<ExamSubjectDto> UpdateSubjectAsync(int examId, int examSubjectId, UpdateExamSubjectDto request);
        Task RemoveSubjectAsync(int examId, int examSubjectId);
    }
}
