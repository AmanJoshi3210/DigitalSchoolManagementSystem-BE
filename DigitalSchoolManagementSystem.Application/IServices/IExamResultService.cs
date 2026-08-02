using DigitalSchoolManagementSystem.Application.DTOs.Academics;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IExamResultService
    {
        Task<ExamResultDto> RecordAsync(RecordExamResultDto request);
        Task<IReadOnlyList<ExamResultDto>> GetByStudentAndExamAsync(int studentId, int examId);
        Task<IReadOnlyList<ExamResultDto>> GetByStudentAsync(int studentId);
    }
}
