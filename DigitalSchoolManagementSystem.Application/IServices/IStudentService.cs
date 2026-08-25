using DigitalSchoolManagementSystem.Application.DTOs.Students;
using DigitalSchoolManagementSystem.Application.Models;
using Microsoft.AspNetCore.Http;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IStudentService
    {
        Task<StudentDto?> GetByIdAsync(int id);
        Task<StudentDto?> GetByUserIdAsync(int userId);
        Task<IReadOnlyList<StudentDto>> GetAllAsync();
        Task<StudentDto> UpdateAsync(int id, UpdateStudentDto request);
        Task<StudentDto> UpdateOwnProfileAsync(int userId, UpdateOwnProfileDto request);
        Task<EducationStatusDto?> GetEducationStatusByIdAsync(int studentId);
        Task<EducationStatusDto?> GetEducationStatusByUserIdAsync(int userId);
        Task DeleteAsync(int id);
        Task<StoredFileResult> UpdateAddOwnProfileImageAsync(IFormFile File, int userId, CancellationToken cancellationToken = default);
    }
}
