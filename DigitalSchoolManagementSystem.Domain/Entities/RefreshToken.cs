using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // Opaque, server-tracked refresh token issued alongside a short-lived JWT access token.
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsValid => !IsRevoked && !IsExpired;
    }
}
