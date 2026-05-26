using BookingCare.Domain.Common;

namespace BookingCare.Domain.Errors
{
    public static class BookingErrors
    {
        public static readonly Error CannotConfirmNonPending =
            new("Booking.CannotConfirmNonPending", "Chỉ có thể xác nhận lịch hẹn đang ở trạng thái Pending.");

        public static readonly Error AlreadyFinalized =
            new("Booking.AlreadyFinalized", "Lịch hẹn đã hoàn thành hoặc đã hủy, không thể thay đổi.");

        public static readonly Error CancellationWindowExpired =
            new("Booking.CancellationWindowExpired", "Chỉ có thể hủy lịch hẹn trước giờ khám ít nhất 2 tiếng.");

        public static readonly Error CannotCompleteNonConfirmed =
            new("Booking.CannotCompleteNonConfirmed", "Chỉ có thể hoàn thành lịch hẹn đang ở trạng thái Confirmed.");

        public static readonly Error CannotRescheduleNonConfirmed =
            new("Booking.CannotRescheduleNonConfirmed", "Chỉ có thể dời lịch khi lịch hẹn đang ở trạng thái Confirmed.");

        public static readonly Error RescheduleWindowExpired =
            new("Booking.RescheduleWindowExpired", "Chỉ có thể dời lịch trước giờ khám ít nhất 2 tiếng.");

        public static readonly Error NotFound =
            new("Booking.NotFound", "Không tìm thấy lịch hẹn.");

        public static readonly Error SlotNotAvailable =
            new("Booking.SlotNotAvailable", "Slot này đã có người đặt hoặc không còn trống.");

        public static readonly Error DuplicateBooking =
            new("Booking.DuplicateBooking", "Bạn đã có lịch hẹn Pending hoặc Confirmed với bác sĩ này trong khung giờ đó.");

        public static readonly Error Unauthorized =
            new("Booking.Unauthorized", "Bạn không có quyền thực hiện thao tác này trên lịch hẹn.");

        public static readonly Error DoctorReasonRequired =
            new("Booking.DoctorReasonRequired", "Bác sĩ hoặc Admin bắt buộc phải nhập lý do hủy.");
    }
}
