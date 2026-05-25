using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.Queries.GetAvailableSlots
{
    public record GetAvailableSlotsQuery(Guid DoctorId, DateOnly Date)
    : IRequest<Result<IReadOnlyList<AvailableSlotDto>>>;
}
