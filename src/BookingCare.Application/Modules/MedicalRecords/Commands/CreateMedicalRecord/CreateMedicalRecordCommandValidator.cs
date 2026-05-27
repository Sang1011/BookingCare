using FluentValidation;

namespace BookingCare.Application.Modules.MedicalRecords.Commands.CreateMedicalRecord
{
    public class CreateMedicalRecordCommandValidator : AbstractValidator<CreateMedicalRecordCommand>
    {
        public CreateMedicalRecordCommandValidator()
        {
            RuleFor(x => x.BookingId).NotEmpty();

            RuleFor(x => x.Diagnosis)
                .NotEmpty().WithMessage("Diagnosis is required.")
                .MaximumLength(2000);

            RuleFor(x => x.Prescription)
                .MaximumLength(2000)
                .When(x => x.Prescription is not null);

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .When(x => x.Notes is not null);
        }
    }
}