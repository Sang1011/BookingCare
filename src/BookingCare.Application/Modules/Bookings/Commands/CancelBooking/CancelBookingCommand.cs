using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.CancelBooking;

public record CancelBookingCommand(
    Guid BookingId,
    string? Reason
) : IRequest<Result>;