using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Email
{
    public class EmailSettings
    {
        public string Host { get; init; } = default!;
        public int Port { get; init; } = 587;
        public string Username { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string FromEmail { get; init; } = default!;
        public string FromName { get; init; } = "BookingCare";
    }
}
