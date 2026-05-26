using FluentValidation;

namespace BookingCare.Application.Modules.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.DoctorScheduleId)
            .NotEmpty().WithMessage("DoctorScheduleId là bắt buộc.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Ghi chú không được vượt quá 1000 ký tự.")
            .When(x => x.Notes is not null);
    }
}