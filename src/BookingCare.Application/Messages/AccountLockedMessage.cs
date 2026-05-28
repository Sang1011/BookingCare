namespace BookingCare.Application.Messages;

public record AccountLockedMessage(
    string Email,
    DateTime LockedUntil
);