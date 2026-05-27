using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecordByBookingId
{
    public class GetMedicalRecordByBookingIdQueryHandler(
        IMedicalRecordRepository medicalRecordRepository,
        ICurrentUser currentUser)
        : IRequestHandler<GetMedicalRecordByBookingIdQuery, Result<MedicalRecordDto>>
    {
        public async Task<Result<MedicalRecordDto>> Handle(
            GetMedicalRecordByBookingIdQuery request,
            CancellationToken cancellationToken)
        {
            var dto = await medicalRecordRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
            if (dto is null)
                return Result<MedicalRecordDto>.Failure(MedicalRecordErrors.NotFound);

            if (currentUser.Role == UserRole.Patient && dto.PatientId != currentUser.UserId)
                return Result<MedicalRecordDto>.Failure(CommonErrors.Unauthorized);

            return Result<MedicalRecordDto>.Success(dto);
        }
    }
}