using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Common
{
    public record Error(string Code, string Message)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
    }
}
