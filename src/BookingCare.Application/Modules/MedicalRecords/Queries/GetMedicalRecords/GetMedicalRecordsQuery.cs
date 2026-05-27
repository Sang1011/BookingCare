using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecords
{
    public record GetMedicalRecordsQuery(
        int Page = 1,
        int PageSize = 10
    ) : IRequest<Result<PagedResult<MedicalRecordDto>>>;
}
