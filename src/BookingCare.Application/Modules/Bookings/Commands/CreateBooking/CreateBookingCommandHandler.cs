// Application/Bookings/Commands/CreateBooking/CreateBookingCommandHandler.cs
using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Booking;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler(
    IBookingRepository bookingRepository,
    IDoctorScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser
) : IRequestHandler<CreateBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var patientId = currentUser.UserId;

        // 1. Load schedule — cần Doctor.Id để check duplicate
        var schedule = await scheduleRepository.GetByIdWithDoctorAsync(request.DoctorScheduleId, ct);
        if (schedule is null)
            return Result<Guid>.Failure(DoctorScheduleErrors.NotFound);

        // 2. Slot còn hiệu lực không?
        if (!schedule.IsAvailable || schedule.IsExpired())
            return Result<Guid>.Failure(BookingErrors.SlotNotAvailable);

        // 3. Duplicate: patient đã đặt với bác sĩ này trong ngày chưa?
        var hasDuplicate = await bookingRepository.HasActiveBookingWithDoctorAsync(
            patientId, schedule.DoctorId, ct);
        if (hasDuplicate)
            return Result<Guid>.Failure(BookingErrors.DuplicateBooking);

        // 4. Conflict: slot đã có Confirmed booking chưa?
        //    Pessimistic lock được handle ở DB level qua transaction Serializable (xem BookingRepository)
        var hasConflict = await bookingRepository.HasConfirmedBookingAsync(request.DoctorScheduleId, ct);
        if (hasConflict)
            return Result<Guid>.Failure(BookingErrors.SlotNotAvailable);

        // 5. Tạo booking
        var booking = Booking.Create(patientId, request.DoctorScheduleId, request.Notes);
        bookingRepository.Add(booking);

        var saveResult = await unitOfWork.SaveChangesAsync(ct);
        if (saveResult.IsFailure)
            return Result<Guid>.Failure(saveResult.Error!);

        return Result<Guid>.Success(booking.Id);
    }
}