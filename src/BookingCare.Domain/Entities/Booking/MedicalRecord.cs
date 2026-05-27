using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;

namespace BookingCare.Domain.Entities.Booking
{
    public class MedicalRecord : AuditableEntity
    {
        public Guid BookingId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public string Diagnosis { get; private set; } = default!;
        public string? Prescription { get; private set; }
        public string? Notes { get; private set; }
        public DateOnly VisitDate { get; private set; }

        public Booking Booking { get; private set; } = default!;

        private MedicalRecord() { }

        public static Result<MedicalRecord> Create(
            Guid bookingId,
            Guid patientId,
            Guid doctorId,
            DateOnly visitDate,
            string diagnosis,
            string? prescription,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                return Result<MedicalRecord>.Failure(MedicalRecordErrors.DiagnosisRequired);

            var record = new MedicalRecord
            {
                BookingId = bookingId,
                PatientId = patientId,
                DoctorId = doctorId,
                VisitDate = visitDate,
                Diagnosis = diagnosis.Trim(),
                Prescription = prescription?.Trim(),
                Notes = notes?.Trim()
            };

            record.Touch();
            return Result<MedicalRecord>.Success(record);
        }

        public Result Update(string diagnosis, string? prescription, string? notes)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                return Result.Failure(MedicalRecordErrors.DiagnosisRequired);

            Diagnosis = diagnosis.Trim();
            Prescription = prescription?.Trim();
            Notes = notes?.Trim();
            Touch();
            return Result.Success();
        }
    }
}