namespace BookingCare.Application.Modules.MedicalRecords.DTOs
{
    public record MedicalRecordDto(
        Guid? Id,
        Guid BookingId,
        Guid PatientId,
        string PatientName,
        Guid DoctorId,
        string DoctorName,
        string SpecialtyName,
        DateOnly VisitDate,
        string Diagnosis,
        string? Treatment,
        string? Notes,
        DateOnly? FollowUpDate,
        IReadOnlyList<PrescriptionItemDto> PrescriptionItems,
        IReadOnlyList<MedicalRecordAttachmentDto> Attachments,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}