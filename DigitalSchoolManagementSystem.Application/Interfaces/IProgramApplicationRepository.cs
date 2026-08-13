using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IProgramApplicationRepository : IGenericRepository<ProgramApplication>
    {
        Task<ProgramApplication?> GetByIdWithDetailsAsync(int id);
        Task<IReadOnlyList<ProgramApplication>> GetByProgramIdAsync(int programId);
        Task<IReadOnlyList<ProgramApplication>> GetByStudentIdAsync(int studentId);
        Task<bool> ExistsForStudentAndProgramAsync(int studentId, int programId);
    }
}
