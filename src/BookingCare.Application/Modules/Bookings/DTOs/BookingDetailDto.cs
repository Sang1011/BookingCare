using BookingCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Bookings.DTOs
{
    public record BookingDetailDto(
        Guid Id,
        Guid PatientId,
        string PatientName,
        string PatientEmail,
        string PatientPhone,
        Guid DoctorId,
        string DoctorName,
        string DoctorEmail,
        string SpecialtyName,
        decimal ConsultationFee,
        DateOnly WorkDate,
        TimeOnly SlotStart,
        TimeOnly SlotEnd,
        BookingStatus Status,
        string? Notes,
        string? CancellationReason,
        string? CancelledBy,
        Guid? RescheduledFromId,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
