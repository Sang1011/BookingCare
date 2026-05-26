using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.ConfirmBooking;

public record ConfirmBookingCommand(Guid BookingId) : IRequest<Result>;