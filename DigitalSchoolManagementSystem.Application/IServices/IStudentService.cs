using DigitalSchoolManagementSystem.Application.DTOs.Students;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IStudentService
    {
        Task<StudentDto?> GetByIdAsync(int id);
        Task<StudentDto?> GetByUserIdAsync(int userId);
        Task<IReadOnlyList<StudentDto>> GetAllAsync();
        Task<StudentDto> UpdateAsync(int id, UpdateStudentDto request);
        Task DeleteAsync(int id);
    }
}
