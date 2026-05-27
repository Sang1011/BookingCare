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

        public static readonly Error Unauthorized =
            new("MedicalRecord.Unauthorized", "You are not authorized to perform this action.");
    }
}