using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingCommandHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser
) : IRequestHandler<ConfirmBookingCommand, Result>
{
    public async Task<Result> Handle(ConfirmBookingCommand request, CancellationToken ct)
    {
        var booking = await bookingRepository.GetByIdWithScheduleAsync(request.BookingId, ct);
        if (booking is null)
            return Result.Failure(BookingErrors.NotFound);

        if (currentUser.Role == UserRole.Doctor && booking.DoctorSchedule.DoctorId != currentUser.UserId)
            return Result.Failure(BookingErrors.Unauthorized);

        var result = booking.Confirm();
        if (result.IsFailure)
            return result;

        return await unitOfWork.SaveChangesAsync(ct);
    }
}