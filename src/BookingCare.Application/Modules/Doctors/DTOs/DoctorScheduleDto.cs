using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.DTOs
{
    public record DoctorScheduleDto(
        Guid Id,
        DateOnly WorkDate,
        TimeOnly SlotStart,
        TimeOnly SlotEnd,
        int MaxPatients,
        bool IsAvailable
    );
}
