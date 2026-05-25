using FluentValidation;

namespace BookingCare.Application.Auth.Commands.RegisterDoctor
{
    public class RegisterDoctorCommandValidator : AbstractValidator<RegisterDoctorCommand>
    {
        public RegisterDoctorCommandValidator()
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

            RuleFor(x => x.LicenseNumber)
                .NotEmpty().WithMessage("Số giấy phép hành nghề không được để trống")
                .MaximumLength(50);

            RuleFor(x => x.SpecialtyId)
                .NotEmpty().WithMessage("Chuyên khoa không được để trống");

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0).WithMessage("Số năm kinh nghiệm không hợp lệ")
                .LessThanOrEqualTo(60).WithMessage("Số năm kinh nghiệm không hợp lệ");

            RuleFor(x => x.ConsultationFee)
                .GreaterThan(0).WithMessage("Phí khám phải lớn hơn 0");

            RuleFor(x => x.Bio)
                .MaximumLength(1000).WithMessage("Bio tối đa 1000 ký tự")
                .When(x => x.Bio is not null);
        }
    }
}