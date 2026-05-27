using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.MedicalRecords.DTOs
{
    public record PrescriptionItemDto(
        Guid? Id,
        string MedicineName,
        string Dosage,
        string Frequency,
        string Duration,
        string? Instructions
    );
}
