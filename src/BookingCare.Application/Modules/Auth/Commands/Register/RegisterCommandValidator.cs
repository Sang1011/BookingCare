using FluentValidation;

namespace BookingCare.Application.Modules.Auth.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Mật khẩu tối thiểu 8 ký tự")
                .Matches("[A-Z]").WithMessage("Phải có ít nhất 1 chữ hoa")
                .Matches("[0-9]").WithMessage("Phải có ít nhất 1 số");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .Matches(@"^[0-9]{10,11}$").WithMessage("Số điện thoại không hợp lệ")
                .When(x => x.Phone is not null);
        }
    }
}
