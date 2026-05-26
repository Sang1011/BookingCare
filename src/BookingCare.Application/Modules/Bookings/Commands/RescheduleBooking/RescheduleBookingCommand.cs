using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.RescheduleBooking;

public record RescheduleBookingCommand(
    Guid BookingId,
    Guid NewDoctorScheduleId
) : IRequest<Result<Guid>>;