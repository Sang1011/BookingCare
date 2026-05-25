using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.Commands.CreateSchedule
{
    public record CreateScheduleCommand(
        Guid DoctorId,
        DateOnly WorkDate,
        TimeOnly SlotStart,
        TimeOnly SlotEnd,
        int MaxPatients = 1
    ) : IRequest<Result<DoctorScheduleDto>>;
}
