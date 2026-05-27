using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.MedicalRecords.Commands.UpdateMedicalRecord
{
    public record UpdateMedicalRecordCommand(
        Guid Id,
        string Diagnosis,
        string? Prescription,
        string? Notes
    ) : IRequest<Result>;
}