using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class FileStorageRepository : GenericRepository<FileStorage>, IFileStorageRepository
    {
        public FileStorageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
