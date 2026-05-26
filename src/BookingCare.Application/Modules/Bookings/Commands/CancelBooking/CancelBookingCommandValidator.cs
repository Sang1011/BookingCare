using FluentValidation;

namespace BookingCare.Application.Modules.Bookings.Commands.CancelBooking;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("BookingId là bắt buộc.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Lý do hủy không được vượt quá 500 ký tự.")
            .When(x => x.Reason is not null);
    }
}