// Application/Bookings/Commands/RescheduleBooking/RescheduleBookingCommandHandler.cs
using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Booking;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.RescheduleBooking;

public class RescheduleBookingCommandHandler(
    IBookingRepository bookingRepository,
    IDoctorScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser
) : IRequestHandler<RescheduleBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RescheduleBookingCommand request, CancellationToken ct)
    {
        var booking = await bookingRepository.GetByIdWithScheduleAsync(request.BookingId, ct);
        if (booking is null)
            return Result<Guid>.Failure(BookingErrors.NotFound);

        if (booking.PatientId != currentUser.UserId)
            return Result<Guid>.Failure(BookingErrors.Unauthorized);

        var newSchedule = await scheduleRepository.GetByIdWithDoctorAsync(request.NewDoctorScheduleId, ct);
        if (newSchedule is null)
            return Result<Guid>.Failure(DoctorScheduleErrors.NotFound);

        if (newSchedule.DoctorId != booking.DoctorSchedule.DoctorId)
            return Result<Guid>.Failure(DoctorScheduleErrors.DoctorMismatch);

        if (!newSchedule.IsAvailable || newSchedule.IsExpired())
            return Result<Guid>.Failure(BookingErrors.SlotNotAvailable);

        var hasConflict = await bookingRepository.HasConfirmedBookingAsync(request.NewDoctorScheduleId, ct);
        if (hasConflict)
            return Result<Guid>.Failure(BookingErrors.SlotNotAvailable);

        var rescheduleResult = booking.Reschedule(request.NewDoctorScheduleId);
        if (rescheduleResult.IsFailure)
            return Result<Guid>.Failure(rescheduleResult.Error!);

        var newBooking = Booking.CreateRescheduled(
            booking.PatientId,
            request.NewDoctorScheduleId,
            booking.Notes,
            booking.Id,
            booking.RescheduleCount,
            booking.DoctorRescheduleCount
        );
        bookingRepository.Add(newBooking);

        var saveResult = await unitOfWork.SaveChangesAsync(ct);
        if (saveResult.IsFailure)
            return Result<Guid>.Failure(saveResult.Error!);

        return Result<Guid>.Success(newBooking.Id);
    }
}