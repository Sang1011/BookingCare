using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Errors
{
    public static class DoctorScheduleErrors
    {
        public static readonly Error NotFound = new("DoctorSchedule.NotFound", "Không tìm thấy lịch làm việc.");
        public static readonly Error InvalidDoctorId = new("DoctorSchedule.InvalidDoctorId", "DoctorId không hợp lệ.");
        public static readonly Error WorkDateInPast = new("DoctorSchedule.WorkDateInPast", "Ngày làm việc không được là ngày trong quá khứ.");
        public static readonly Error InvalidSlotRange = new("DoctorSchedule.InvalidSlotRange", "Giờ kết thúc phải sau giờ bắt đầu.");
        public static readonly Error InvalidMaxPatients = new("DoctorSchedule.InvalidMaxPatients", "Số bệnh nhân tối đa phải lớn hơn 0.");
        public static readonly Error SlotOutsideWorkingHours = new("DoctorSchedule.SlotOutsideWorkingHours", "Slot phải trong giờ làm việc (7:00 - 17:00).");
        public static readonly Error SlotConflict = new("DoctorSchedule.SlotConflict", "Bác sĩ đã có lịch làm việc trong khung giờ này.");
        public static readonly Error HasActiveBookings = new("DoctorSchedule.HasActiveBookings", "Không thể xóa lịch đã có booking.");
        public static readonly Error SlotExpired = new("DoctorSchedule.SlotExpired", "Slot này đã qua giờ hiện tại.");
        public static readonly Error SlotUnavailable = new("DoctorSchedule.SlotUnavailable", "Slot này không còn khả dụng.");
        public static readonly Error DoctorMismatch = new("DoctorSchedule.DoctorMismatch", "Slot mới phải thuộc cùng bác sĩ với lịch hẹn gốc.");
    }
}
