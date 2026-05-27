using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecords
{
    public class GetMedicalRecordsQueryHandler(
    IMedicalRecordRepository medicalRecordRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetMedicalRecordsQuery, Result<PagedResult<MedicalRecordDto>>>
    {
        public async Task<Result<PagedResult<MedicalRecordDto>>> Handle(
            GetMedicalRecordsQuery request, CancellationToken cancellationToken)
        {
            var patientId = currentUser.Role == UserRole.Patient
                ? currentUser.UserId
                : (Guid?)null;

            var (items, total) = await medicalRecordRepository.GetPagedByPatientAsync(
                patientId ?? Guid.Empty,
                request.Page,
                request.PageSize,
                cancellationToken);

            return Result<PagedResult<MedicalRecordDto>>.Success(
                new PagedResult<MedicalRecordDto>(items, total, request.Page, request.PageSize));
        }
    }
}
