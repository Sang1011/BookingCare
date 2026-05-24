using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;

namespace BookingCare.Domain.Entities.Auth
{
    public class User : AuditableEntity
    {
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public string FullName { get; private set; } = default!;
        public string? Phone { get; private set; }
        public DateOnly? DateOfBirth { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; } = true;
        public int FailedLoginCount { get; private set; }
        public DateTime? LockedUntil { get; private set; }

        private User() { }

        public static User Create(
        string email, string passwordHash,
        string fullName, string? phone,
        DateOnly? dateOfBirth, UserRole role = UserRole.Patient)
        {
            return new User
            {
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FullName = fullName,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Role = role
            };
        }

        public bool IsLocked() =>
        LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;

        public void RecordFailedLogin()
        {
            FailedLoginCount++;
            if (FailedLoginCount >= 5)
                LockedUntil = DateTime.UtcNow.AddMinutes(15);
            Touch();
        }

        public void ResetFailedLogin()
        {
            FailedLoginCount = 0;
            LockedUntil = null;
            Touch();
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            Touch();
        }
    }
}
