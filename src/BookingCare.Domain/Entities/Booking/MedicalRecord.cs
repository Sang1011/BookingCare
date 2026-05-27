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
        public string? Treatment { get; private set; }
        public string? Notes { get; private set; }
        public DateOnly VisitDate { get; private set; }
        public DateOnly? FollowUpDate { get; private set; }

        private readonly List<PrescriptionItem> _prescriptionItems = [];
        public IReadOnlyList<PrescriptionItem> PrescriptionItems => _prescriptionItems.AsReadOnly();

        private readonly List<MedicalRecordAttachment> _attachments = [];
        public IReadOnlyList<MedicalRecordAttachment> Attachments => _attachments.AsReadOnly();

        public Booking Booking { get; private set; } = default!;

        private MedicalRecord() { }

        public static Result<MedicalRecord> Create(
            Guid bookingId,
            Guid patientId,
            Guid doctorId,
            DateOnly visitDate,
            string diagnosis,
            string? treatment,
            string? notes,
            DateOnly? followUpDate)
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
                Treatment = treatment?.Trim(),
                Notes = notes?.Trim(),
                FollowUpDate = followUpDate
            };

            record.Touch();
            return Result<MedicalRecord>.Success(record);
        }

        public bool CanEdit() => (DateTime.UtcNow - CreatedAt).TotalHours <= 24;

        public Result Update(
            string diagnosis,
            string? treatment,
            string? notes,
            DateOnly? followUpDate)
        {
            if (!CanEdit())
                return Result.Failure(MedicalRecordErrors.EditWindowExpired);

            if (string.IsNullOrWhiteSpace(diagnosis))
                return Result.Failure(MedicalRecordErrors.DiagnosisRequired);

            Diagnosis = diagnosis.Trim();
            Treatment = treatment?.Trim();
            Notes = notes?.Trim();
            FollowUpDate = followUpDate;
            Touch();
            return Result.Success();
        }

        public void ReplacePrescriptionItems(IEnumerable<PrescriptionItem> items)
        {
            _prescriptionItems.Clear();
            _prescriptionItems.AddRange(items);
        }

        public void AddPrescriptionItem(PrescriptionItem item)
            => _prescriptionItems.Add(item);

        public void AddAttachment(MedicalRecordAttachment attachment)
            => _attachments.Add(attachment);

        public Result RemoveAttachment(Guid attachmentId)
        {
            var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
            if (attachment is null)
                return Result.Failure(MedicalRecordErrors.AttachmentNotFound);

            _attachments.Remove(attachment);
            return Result.Success();
        }
    }
}