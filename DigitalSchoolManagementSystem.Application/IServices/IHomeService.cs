using DigitalSchoolManagementSystem.Application.DTOs.Home;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IHomeService
    {
        Task<HomeDto?> GetHomeAsync(int userId);
    }
}
