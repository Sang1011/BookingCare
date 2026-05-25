using BookingCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Security
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        string Email { get; }
        UserRole Role { get; }
        bool IsAuthenticated { get; }
    }
}
