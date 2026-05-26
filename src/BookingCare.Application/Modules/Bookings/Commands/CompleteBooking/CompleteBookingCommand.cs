using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.CompleteBooking;

public record CompleteBookingCommand(Guid BookingId) : IRequest<Result>;