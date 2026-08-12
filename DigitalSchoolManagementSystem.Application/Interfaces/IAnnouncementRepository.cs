using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IAnnouncementRepository : IGenericRepository<Announcement>
    {
        Task<IReadOnlyList<Announcement>> GetByTargetGradeAsync(string targetGrade);
    }
}
