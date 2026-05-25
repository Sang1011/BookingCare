using BookingCare.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Guid? GetUserIdFromExpiredToken(string token);
    }
}
