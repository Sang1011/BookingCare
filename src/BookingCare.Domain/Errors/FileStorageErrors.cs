using BookingCare.Domain.Common;

namespace BookingCare.Domain.Errors;

public static class FileStorageErrors
{
    public static readonly Error EmptyFile =
        new("FileStorage.EmptyFile", "File không được rỗng.");

    public static readonly Error FileTooLarge =
        new("FileStorage.FileTooLarge", "File vượt quá giới hạn cho phép (10 MB).");

    public static readonly Error InvalidContentType =
        new("FileStorage.InvalidContentType", "Định dạng file không được hỗ trợ. Chỉ chấp nhận: JPG, PNG, WEBP, PDF, DOC, DOCX.");

    public static readonly Error InvalidExtension =
        new("FileStorage.InvalidExtension", "Phần mở rộng file không hợp lệ.");

    public static readonly Error InvalidPath =
        new("FileStorage.InvalidPath", "Đường dẫn file không hợp lệ.");

    public static readonly Error FileNotFound =
        new("FileStorage.FileNotFound", "Không tìm thấy file cần xóa.");

    public static readonly Error UploadFailed =
        new("FileStorage.UploadFailed", "Upload file thất bại. Vui lòng thử lại.");

    public static readonly Error DeleteFailed =
        new("FileStorage.DeleteFailed", "Xóa file thất bại. Vui lòng thử lại.");
}