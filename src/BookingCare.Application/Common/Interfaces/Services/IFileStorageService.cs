using BookingCare.Domain.Common;

namespace BookingCare.Application.Common.Interfaces.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Upload file từ Stream. Dùng khi cần control thủ công.
    /// </summary>
    Task<Result<string>> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xóa file theo relative URL đã lưu trong DB (vd: /uploads/2025/06/abc.pdf)
    /// </summary>
    Task<Result> DeleteAsync(string fileUrl, CancellationToken cancellationToken = default);
}