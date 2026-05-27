using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Domain.Common;
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
            var record = await medicalRecordRepository.GetByIdAsync(request.Id, cancellationToken);
            if (record is null)
                return Result.Failure(MedicalRecordErrors.NotFound);

            if (record.DoctorId != currentUser.UserId && currentUser.Role != UserRole.Admin)
                return Result.Failure(CommonErrors.Unauthorized);

            var result = record.Update(request.Diagnosis, request.Prescription, request.Notes);
            if (result.IsFailure) return result;

            medicalRecordRepository.Update(record);
            return await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}