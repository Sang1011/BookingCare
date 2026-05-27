using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecordById
{
    public class GetMedicalRecordByIdQueryHandler(
    IMedicalRecordRepository medicalRecordRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetMedicalRecordByIdQuery, Result<MedicalRecordDto>>
    {
        public async Task<Result<MedicalRecordDto>> Handle(
            GetMedicalRecordByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await medicalRecordRepository.GetDtoByIdAsync(request.Id, cancellationToken);
            if (dto is null)
                return Result<MedicalRecordDto>.Failure(MedicalRecordErrors.NotFound);

            if (currentUser.Role == UserRole.Patient && dto.PatientId != currentUser.UserId)
                return Result<MedicalRecordDto>.Failure(CommonErrors.Unauthorized);

            return Result<MedicalRecordDto>.Success(dto);
        }
    }
}
