using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Errors
{
    public static class DoctorErrors
    {
        public static readonly Error NotFound =
            new("Doctor.NotFound", "Không tìm thấy bác sĩ");

        public static readonly Error AlreadyExists =
            new("Doctor.AlreadyExists", "User này đã là bác sĩ");

        public static readonly Error NotAuthorized =
            new("Doctor.NotAuthorized", "Bạn không có quyền thực hiện thao tác này");
    }
}
