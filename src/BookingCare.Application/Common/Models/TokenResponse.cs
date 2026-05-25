using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Models
{
    public record TokenResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt
    );
}
