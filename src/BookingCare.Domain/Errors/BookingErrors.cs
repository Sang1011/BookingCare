using BookingCare.Domain.Common;

namespace BookingCare.Domain.Errors
{
    public static class BookingErrors
    {
        public static readonly Error CannotConfirmNonPending =
            new("Booking.CannotConfirm", "Chỉ có thể confirm booking đang Pending");

        public static readonly Error AlreadyFinalized =
            new("Booking.AlreadyFinalized", "Booking đã kết thúc, không thể thay đổi");

        public static readonly Error CancellationWindowExpired =
            new("Booking.CancellationWindowExpired", "Phải hủy trước giờ khám ít nhất 2 tiếng");

        public static readonly Error CannotCompleteNonConfirmed =
            new("Booking.CannotComplete", "Chỉ có thể hoàn thành booking đã Confirmed");

        public static readonly Error SlotUnavailable =
            new("Booking.SlotUnavailable", "Slot này đã được đặt");

        public static readonly Error NotFound =
            new("Booking.NotFound", "Không tìm thấy lịch hẹn");
    }
}
