using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.CancelBooking;

public class CancelBookingCommandHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser
) : IRequestHandler<CancelBookingCommand, Result>
{
    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var booking = await bookingRepository.GetByIdWithScheduleAsync(request.BookingId, ct);
        if (booking is null)
            return Result.Failure(BookingErrors.NotFound);

        var cancelledBy = currentUser.Role switch
        {
            UserRole.Admin => CancelledBy.Admin,
            UserRole.Doctor => CancelledBy.Doctor,
            _ => CancelledBy.Patient
        };

        if (cancelledBy == CancelledBy.Patient && booking.PatientId != currentUser.UserId)
            return Result.Failure(BookingErrors.Unauthorized);

        if (cancelledBy == CancelledBy.Doctor && booking.DoctorSchedule.DoctorId != currentUser.UserId)
            return Result.Failure(BookingErrors.Unauthorized);

        if (cancelledBy != CancelledBy.Patient && string.IsNullOrWhiteSpace(request.Reason))
            return Result.Failure(BookingErrors.DoctorReasonRequired);

        var reason = request.Reason ?? "Hủy bởi bệnh nhân";
        var result = booking.Cancel(reason, cancelledBy);
        if (result.IsFailure)
            return result;

        return await unitOfWork.SaveChangesAsync(ct);
    }
}