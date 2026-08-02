using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface ISubjectRepository : IGenericRepository<Subject>
    {
        Task<IReadOnlyList<Subject>> GetByGradeAsync(string grade);
        Task<bool> ExistsForGradeAsync(string grade, string code);
    }
}
