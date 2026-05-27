namespace BookingCare.Application.Modules.MedicalRecords.DTOs
{
    public record MedicalRecordDto(
        Guid Id,
        Guid BookingId,
        Guid PatientId,
        string PatientName,
        Guid DoctorId,
        string DoctorName,
        string SpecialtyName,
        DateOnly VisitDate,
        string Diagnosis,
        string? Prescription,
        string? Notes,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}