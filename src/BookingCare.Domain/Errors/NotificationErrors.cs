using BookingCare.Domain.Common;

namespace BookingCare.Domain.Errors;

public static class NotificationErrors
{
    public static readonly Error NoRecipientsProvided = new(
        "Notification.NoRecipientsProvided",
        "Danh sách người nhận không được để trống khi SendToAll = false");
}