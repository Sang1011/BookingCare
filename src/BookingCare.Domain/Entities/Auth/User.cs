using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Events;
using BookingCare.Domain.ValueObjects;

namespace BookingCare.Domain.Entities.Auth
{
    public class User : AuditableEntity
    {
        public Email Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public string FullName { get; private set; } = default!;
        public string? Phone { get; private set; }
        public DateOnly? DateOfBirth { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; } = false;

        private User() { }

        public static Result<User> Create(   
            string email, string passwordHash,
            string fullName, string? phone,
            DateOnly? dateOfBirth, UserRole role = UserRole.Patient)
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
                return Result<User>.Failure(emailResult.Error!);

            var user = new User
            {
                Email = emailResult.Value!,
                PasswordHash = passwordHash,
                FullName = fullName,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Role = role
            };

            user.RaiseDomainEvent(new EmailVerificationRequestedEvent(user.Id, user.Email.Value, string.Empty));
            return Result<User>.Success(user);
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
