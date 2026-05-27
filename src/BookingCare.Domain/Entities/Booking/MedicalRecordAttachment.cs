using BookingCare.Domain.Common;

namespace BookingCare.Domain.Entities.Booking
{
    public class MedicalRecordAttachment : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        public string FileName { get; private set; } = default!;
        public string FileUrl { get; private set; } = default!;
        public long FileSize { get; private set; }
        public string ContentType { get; private set; } = default!;
        public DateTime UploadedAt { get; private set; }

        public MedicalRecord MedicalRecord { get; private set; } = default!;

        private MedicalRecordAttachment() { }

        public static MedicalRecordAttachment Create(
            Guid medicalRecordId,
            string fileName,
            string fileUrl,
            long fileSize,
            string contentType)
        {
            return new MedicalRecordAttachment
            {
                MedicalRecordId = medicalRecordId,
                FileName = fileName.Trim(),
                FileUrl = fileUrl.Trim(),
                FileSize = fileSize,
                ContentType = contentType.Trim(),
                UploadedAt = DateTime.UtcNow
            };
        }
    }
}