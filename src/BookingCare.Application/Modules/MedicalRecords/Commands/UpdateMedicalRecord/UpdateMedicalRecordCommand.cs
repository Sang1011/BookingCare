using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.MedicalRecords.Commands.UpdateMedicalRecord
{
    public record UpdateMedicalRecordCommand(
        Guid Id,
        string Diagnosis,
        string? Treatment,
        string? Notes,
        DateOnly? FollowUpDate,
        List<PrescriptionItemDto>? PrescriptionItems
    ) : IRequest<Result>;
}