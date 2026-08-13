using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IEducationProgramRepository : IGenericRepository<EducationProgram>
    {
        Task<IReadOnlyList<EducationProgram>> GetActiveAsync();
        Task<IReadOnlyList<EducationProgram>> GetAllOrderedAsync();
    }
}
