using BookingCare.Domain.Common;

namespace BookingCare.Domain.Errors
{
    public static class MedicalRecordErrors
    {
        public static readonly Error NotFound =
            new("MedicalRecord.NotFound", "Medical record not found.");

        public static readonly Error DiagnosisRequired =
            new("MedicalRecord.DiagnosisRequired", "Diagnosis is required.");

        public static readonly Error BookingNotCompleted =
            new("MedicalRecord.BookingNotCompleted", "Medical record can only be created for completed bookings.");

        public static readonly Error AlreadyExists =
            new("MedicalRecord.AlreadyExists", "A medical record already exists for this booking.");

        public static readonly Error EditWindowExpired =
            new("MedicalRecord.EditWindowExpired", "Chỉ có thể chỉnh sửa hồ sơ trong vòng 24h sau khi tạo.");

        public static readonly Error AttachmentNotFound =
            new("MedicalRecord.AttachmentNotFound", "Không tìm thấy file đính kèm.");
    }
}