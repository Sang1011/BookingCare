using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.Commands.CreateSchedule
{
    public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
    {
        public CreateScheduleCommandValidator()
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("DoctorId không được để trống.");

            RuleFor(x => x.WorkDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
                .WithMessage("Ngày làm việc không được là ngày trong quá khứ.");

            RuleFor(x => x.SlotStart)
                .GreaterThanOrEqualTo(new TimeOnly(7, 0))
                .WithMessage("Slot bắt đầu không được trước 7:00.");

            RuleFor(x => x.SlotEnd)
                .LessThanOrEqualTo(new TimeOnly(17, 0))
                .WithMessage("Slot kết thúc không được sau 17:00.")
                .GreaterThan(x => x.SlotStart)
                .WithMessage("Giờ kết thúc phải sau giờ bắt đầu.");

            RuleFor(x => x.MaxPatients)
                .GreaterThan(0).WithMessage("Số bệnh nhân tối đa phải lớn hơn 0.");
        }
    }
}
