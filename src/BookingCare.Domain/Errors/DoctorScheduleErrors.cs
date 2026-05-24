using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Errors
{
    public static class DoctorScheduleErrors
    {
        public static readonly Error InvalidSlotTime =
            new("DoctorSchedule.InvalidSlotTime", "SlotEnd phải sau SlotStart");

        public static readonly Error AlreadyExpired =
            new("DoctorSchedule.AlreadyExpired", "Slot này đã qua giờ khám");

        public static readonly Error HasExistingBooking =
            new("DoctorSchedule.HasExistingBooking", "Không thể xóa slot đã có booking");

        public static readonly Error NotFound =
            new("DoctorSchedule.NotFound", "Không tìm thấy lịch làm việc");

        public static readonly Error DuplicateSlot =
            new("DoctorSchedule.DuplicateSlot", "Bác sĩ đã có lịch trong khung giờ này");
    }
}
