namespace BookingCare.Application.Messages;

public record BookingConfirmedMessage(
    Guid BookingId,
    Guid PatientId,
    string PatientEmail,
    string PatientName,
    string DoctorName,
    DateOnly WorkDate,
    TimeOnly SlotStart,
    TimeOnly SlotEnd
);