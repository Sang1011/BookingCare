using FluentValidation;

namespace BookingCare.Application.Modules.MedicalRecords.Commands.UpdateMedicalRecord
{
    public class UpdateMedicalRecordCommandValidator : AbstractValidator<UpdateMedicalRecordCommand>
    {
        public UpdateMedicalRecordCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Diagnosis)
                .NotEmpty().WithMessage("Diagnosis is required.")
                .MaximumLength(2000);

            RuleFor(x => x.Treatment)
                .MaximumLength(2000)
                .When(x => x.Treatment is not null);

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .When(x => x.Notes is not null);

            RuleFor(x => x.FollowUpDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .When(x => x.FollowUpDate.HasValue)
                .WithMessage("Follow-up date cannot be in the past.");

            RuleForEach(x => x.PrescriptionItems)
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.MedicineName)
                        .NotEmpty().WithMessage("Medicine name is required.")
                        .MaximumLength(200);

                    item.RuleFor(x => x.Dosage)
                        .NotEmpty().WithMessage("Dosage is required.")
                        .MaximumLength(100);

                    item.RuleFor(x => x.Frequency)
                        .NotEmpty().WithMessage("Frequency is required.")
                        .MaximumLength(100);

                    item.RuleFor(x => x.Duration)
                        .NotEmpty().WithMessage("Duration is required.")
                        .MaximumLength(100);

                    item.RuleFor(x => x.Instructions)
                        .MaximumLength(500)
                        .When(x => x.Instructions is not null);
                });
        }
    }
}