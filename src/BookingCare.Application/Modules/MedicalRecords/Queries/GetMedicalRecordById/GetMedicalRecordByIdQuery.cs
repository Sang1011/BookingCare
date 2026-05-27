using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecordById
{
    public record GetMedicalRecordByIdQuery(Guid Id) : IRequest<Result<MedicalRecordDto>>;
}

