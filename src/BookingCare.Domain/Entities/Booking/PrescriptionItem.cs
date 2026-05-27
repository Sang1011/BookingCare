using BookingCare.Domain.Common;

namespace BookingCare.Domain.Entities.Booking
{
    public class PrescriptionItem : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        public string MedicineName { get; private set; } = default!;
        public string Dosage { get; private set; } = default!;
        public string Frequency { get; private set; } = default!;
        public string Duration { get; private set; } = default!;
        public string? Instructions { get; private set; }

        public MedicalRecord MedicalRecord { get; private set; } = default!;

        private PrescriptionItem() { }

        public static PrescriptionItem Create(
            Guid medicalRecordId,
            string medicineName,
            string dosage,
            string frequency,
            string duration,
            string? instructions)
        {
            return new PrescriptionItem
            {
                MedicalRecordId = medicalRecordId,
                MedicineName = medicineName.Trim(),
                Dosage = dosage.Trim(),
                Frequency = frequency.Trim(),
                Duration = duration.Trim(),
                Instructions = instructions?.Trim()
            };
        }
    }
}