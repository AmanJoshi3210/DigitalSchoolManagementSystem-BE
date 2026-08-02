using DigitalSchoolManagementSystem.Domain.Common;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // Login credentials + fields shared by both Students and StaffUsers.
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfileImageUrl { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public Student? Student { get; set; }
        public StaffUser? StaffUser { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
