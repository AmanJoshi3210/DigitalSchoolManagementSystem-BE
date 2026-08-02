using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IExamResultRepository : IGenericRepository<ExamResult>
    {
        Task<ExamResult?> GetByStudentAndExamSubjectAsync(int studentId, int examSubjectId);
        Task<IReadOnlyList<ExamResult>> GetByStudentAndExamAsync(int studentId, int examId);
        Task<IReadOnlyList<ExamResult>> GetByStudentAsync(int studentId);
    }
}
