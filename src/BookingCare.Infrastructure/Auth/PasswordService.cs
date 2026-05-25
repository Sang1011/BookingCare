using BookingCare.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Auth
{
    public class PasswordService : IPasswordService
    {
        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
