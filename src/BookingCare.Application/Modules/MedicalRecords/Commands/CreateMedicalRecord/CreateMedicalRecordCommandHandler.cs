using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Modules.MedicalRecords.Commands.CreateMedicalRecord;
using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Booking;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

public class CreateMedicalRecordCommandHandler(
    IBookingRepository bookingRepository,
    IMedicalRecordRepository medicalRecordRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateMedicalRecordCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(
        CreateMedicalRecordCommand request,
        CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetByIdWithScheduleAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<MedicalRecordDto>.Failure(BookingErrors.NotFound);

        var doctorId = booking.DoctorSchedule.DoctorId;

        if (doctorId != currentUser.UserId && !currentUser.Role.Equals(UserRole.Admin.ToString()))
            return Result<MedicalRecordDto>.Failure(MedicalRecordErrors.Unauthorized);

        if (booking.Status != BookingStatus.Completed)
            return Result<MedicalRecordDto>.Failure(MedicalRecordErrors.BookingNotCompleted);

        var exists = await medicalRecordRepository.ExistsForBookingAsync(request.BookingId, cancellationToken);
        if (exists)
            return Result<MedicalRecordDto>.Failure(MedicalRecordErrors.AlreadyExists);

        var result = MedicalRecord.Create(
            bookingId: request.BookingId,
            patientId: booking.PatientId,
            doctorId: doctorId,
            visitDate: booking.DoctorSchedule.WorkDate,
            diagnosis: request.Diagnosis,
            prescription: request.Prescription,
            notes: request.Notes);

        if (result.IsFailure)
            return Result<MedicalRecordDto>.Failure(result.Error!);

        medicalRecordRepository.Add(result.Value!);
        var saveResult = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return Result<MedicalRecordDto>.Failure(saveResult.Error!);

        var dto = await medicalRecordRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        return Result<MedicalRecordDto>.Success(dto!);
    }
}