using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Events;

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
        public bool IsActive { get; private set; } = false;

        private User() { }

        public static User Create(
            string email, string passwordHash,
            string fullName, string? phone,
            DateOnly? dateOfBirth, UserRole role = UserRole.Patient)
        {
            var user = new User
            {
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FullName = fullName,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Role = role
            };

            user.RaiseDomainEvent(new EmailVerificationRequestedEvent(user.Id, user.Email, string.Empty));
            return user;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            Touch();
        }

        public void Activate()
        {
            IsActive = true;
            Touch();
        }
    }
}
