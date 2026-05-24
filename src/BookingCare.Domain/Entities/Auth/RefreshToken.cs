using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Entities.Auth
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = default!;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string hashedToken)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }

        public bool IsValid() => !IsRevoked && ExpiresAt > DateTime.UtcNow;

        public void Revoke()
        {
            IsRevoked = true;
        }
    }
}
