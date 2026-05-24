using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Enums
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled,
        Rescheduled
    }
}
