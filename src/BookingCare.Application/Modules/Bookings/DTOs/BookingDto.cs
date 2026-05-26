using BookingCare.Domain.Enums;

namespace BookingCare.Application.Modules.Bookings.DTOs
{
    public record BookingDto(
        Guid Id,
        Guid PatientId,
        string PatientName,
        Guid DoctorId,
        string DoctorName,
        string SpecialtyName,
        DateOnly WorkDate,
        TimeOnly SlotStart,
        TimeOnly SlotEnd,
        BookingStatus Status,
        string? Notes,
        DateTime CreatedAt
    );
}
