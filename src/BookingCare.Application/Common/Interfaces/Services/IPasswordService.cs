using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Services
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
