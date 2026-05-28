namespace BookingCare.Application.Messages;

public record EmailVerificationRequestedMessage(
    Guid UserId,
    string Email,
    string Token
);