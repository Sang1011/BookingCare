using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Booking;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.MedicalRecords.Commands.UpdateMedicalRecord
{
    public class UpdateMedicalRecordCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateMedicalRecordCommand, Result>
    {
        public async Task<Result> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var record = await medicalRecordRepository.GetEntityByIdAsync(request.Id, cancellationToken);

            if (record is null)
                return Result.Failure(MedicalRecordErrors.NotFound);

            if (record.DoctorId != currentUser.UserId && currentUser.Role != UserRole.Admin)
                return Result.Failure(CommonErrors.Unauthorized);

            var result = record.Update(
                diagnosis: request.Diagnosis,
                treatment: request.Treatment,
                notes: request.Notes,
                followUpDate: request.FollowUpDate);

            if (result.IsFailure)
                return result;

            if (request.PrescriptionItems is not null)
            {
                var newItems = request.PrescriptionItems.Select(item =>
                    PrescriptionItem.Create(
                        medicalRecordId: record.Id,
                        medicineName: item.MedicineName,
                        dosage: item.Dosage,
                        frequency: item.Frequency,
                        duration: item.Duration,
                        instructions: item.Instructions));

                medicalRecordRepository.ReplacePrescriptionItems(record, newItems);
            }

            medicalRecordRepository.Update(record);

            return await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}