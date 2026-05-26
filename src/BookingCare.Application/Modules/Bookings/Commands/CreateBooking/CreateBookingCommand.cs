using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(
    Guid DoctorScheduleId,
    string? Notes
) : IRequest<Result<Guid>>;