using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetByUserIdAsync(int userId);
        Task<Student?> GetByAdmissionNumberAsync(string admissionNumber);
        Task<Student?> GetByIdWithDetailsAsync(int id);
        Task<IReadOnlyList<Student>> GetAllWithDetailsAsync();
    }
}
