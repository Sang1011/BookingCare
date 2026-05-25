using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Errors
{
    public static class DoctorErrors
    {
        public static readonly Error NotFound = new("Doctor.NotFound", "Không tìm thấy bác sĩ.");
        public static readonly Error AlreadyExists = new("Doctor.AlreadyExists", "Bác sĩ đã được đăng ký.");
        public static readonly Error InvalidUserId = new("Doctor.InvalidUserId", "UserId không hợp lệ.");
        public static readonly Error LicenseNumberRequired = new("Doctor.LicenseNumberRequired", "Số chứng chỉ hành nghề không được để trống.");
        public static readonly Error LicenseNumberDuplicate = new("Doctor.LicenseNumberDuplicate", "Số chứng chỉ hành nghề đã được sử dụng.");
        public static readonly Error InvalidYearsOfExperience = new("Doctor.InvalidYearsOfExperience", "Số năm kinh nghiệm không hợp lệ.");
        public static readonly Error InvalidConsultationFee = new("Doctor.InvalidConsultationFee", "Phí khám không hợp lệ.");
        public static readonly Error NotVerified = new("Doctor.NotVerified", "Tài khoản bác sĩ chưa được xác minh");
        public static readonly Error AlreadyVerified = new("Doctor.AlreadyVerified", "Tài khoản bác sĩ đã được xác minh");
    }
}
