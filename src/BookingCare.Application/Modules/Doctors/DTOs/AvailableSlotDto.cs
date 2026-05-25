using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.DTOs
{
    public record AvailableSlotDto(
        Guid ScheduleId,
        TimeOnly SlotStart,
        TimeOnly SlotEnd,
        decimal ConsultationFee
    );
}
