using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Errors
{
    public static class UserErrors
    {
        public static readonly Error NotFound =
            new("User.NotFound", "Không tìm thấy người dùng");

        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "Email hoặc mật khẩu không đúng");

        public static readonly Error AccountLocked =
            new("User.AccountLocked", "Tài khoản đang bị tạm khóa, vui lòng thử lại sau");

        public static readonly Error EmailAlreadyExists =
            new("User.EmailAlreadyExists", "Email này đã được sử dụng");

        public static readonly Error Inactive =
            new("User.Inactive", "Tài khoản không còn hoạt động");

        public static readonly Error InvalidVerificationToken =
            new("User.InvalidVerificationToken", "Token xác nhận không hợp lệ hoặc đã hết hạn");

        public static readonly Error EmailRequired = new("User.EmailRequired", "Email is required.");
        public static readonly Error InvalidEmailFormat = new("User.InvalidEmailFormat", "Email format is invalid.");
    }
}
