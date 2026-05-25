using BookingCare.Domain.Common;

namespace BookingCare.Domain.Errors;

public static class SpecialtyErrors
{
    public static readonly Error NameRequired = new("Specialty.NameRequired", "Tên chuyên khoa không được để trống.");
    public static readonly Error NameTooLong = new("Specialty.NameTooLong", "Tên chuyên khoa không được vượt quá 200 ký tự.");
    public static readonly Error NotFound = new("Specialty.NotFound", "Không tìm thấy chuyên khoa.");
    public static readonly Error AlreadyExists = new("Specialty.AlreadyExists", "Chuyên khoa này đã tồn tại.");
}