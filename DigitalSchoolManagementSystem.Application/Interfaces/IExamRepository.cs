using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IExamRepository : IGenericRepository<Exam>
    {
        Task<Exam?> GetByIdWithSubjectsAsync(int id);
        Task<IReadOnlyList<Exam>> SearchAsync(string? grade, string? academicYear);
        Task<ExamSubject?> GetExamSubjectAsync(int examSubjectId);
        Task<bool> SubjectAlreadyScheduledAsync(int examId, int subjectId);
    }
}
