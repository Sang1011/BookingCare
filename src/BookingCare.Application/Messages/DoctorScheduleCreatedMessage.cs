namespace BookingCare.Application.Messages;

public record DoctorScheduleCreatedMessage(
    Guid DoctorId,
    string DoctorEmail,
    string DoctorName,
    DateOnly WorkDate,
    TimeOnly SlotStart,
    TimeOnly SlotEnd
);