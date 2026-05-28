namespace BookingCare.Application.Messages;

public record SuspiciousLoginAttemptMessage(
    string Email
);