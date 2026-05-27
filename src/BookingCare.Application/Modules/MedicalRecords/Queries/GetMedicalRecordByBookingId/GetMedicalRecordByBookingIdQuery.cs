using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecordByBookingId
{
    public record GetMedicalRecordByBookingIdQuery(Guid BookingId)
        : IRequest<Result<MedicalRecordDto>>;
}