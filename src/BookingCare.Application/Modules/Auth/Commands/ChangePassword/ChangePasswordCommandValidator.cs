using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Phải có ít nhất 1 chữ hoa")
                .Matches("[0-9]").WithMessage("Phải có ít nhất 1 số")
                .NotEqual(x => x.CurrentPassword)
                    .WithMessage("Mật khẩu mới phải khác mật khẩu cũ");
        }
    }
}
