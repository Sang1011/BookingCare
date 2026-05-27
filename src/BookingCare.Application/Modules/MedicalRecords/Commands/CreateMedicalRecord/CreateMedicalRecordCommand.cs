using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.MedicalRecords.Commands.CreateMedicalRecord
{
    public record CreateMedicalRecordCommand(
        Guid BookingId,
        string Diagnosis,
        string? Treatment,
        string? Notes,
        DateOnly? FollowUpDate,
        List<PrescriptionItemDto>? PrescriptionItems
    ) : IRequest<Result<MedicalRecordDto>>;
}