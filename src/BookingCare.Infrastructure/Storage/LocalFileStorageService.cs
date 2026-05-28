using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookingCare.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp",
        ".pdf",
        ".doc", ".docx"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024;
    private readonly string _webRootPath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IWebHostEnvironment env,
        ILogger<LocalFileStorageService> logger)
    {
        _webRootPath = env.WebRootPath
            ?? throw new InvalidOperationException("WebRootPath is not configured. Ensure UseStaticFiles() is set up.");
        _logger = logger;
    }

    public async Task<Result<string>> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var validationResult = ValidateFile(stream.Length, contentType, fileName);
        if (validationResult.IsFailure)
            return Result<string>.Failure(validationResult.Error!);

        return await SaveAsync(stream, fileName, cancellationToken);
    }

    public Task<Result> DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var absolutePath = Path.Combine(_webRootPath, relativePath);

            if (!absolutePath.StartsWith(_webRootPath, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Attempted path traversal on delete: {FileUrl}", fileUrl);
                return Task.FromResult(Result.Failure(FileStorageErrors.InvalidPath));
            }

            if (!File.Exists(absolutePath))
            {
                _logger.LogWarning("File not found for deletion: {AbsolutePath}", absolutePath);
                return Task.FromResult(Result.Failure(FileStorageErrors.FileNotFound));
            }

            File.Delete(absolutePath);
            _logger.LogInformation("Deleted file: {FileUrl}", fileUrl);

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {FileUrl}", fileUrl);
            return Task.FromResult(Result.Failure(FileStorageErrors.DeleteFailed));
        }
    }

    private static Result ValidateFile(long sizeBytes, string contentType, string fileName)
    {
        if (sizeBytes == 0)
            return Result.Failure(FileStorageErrors.EmptyFile);

        if (sizeBytes > MaxFileSizeBytes)
            return Result.Failure(FileStorageErrors.FileTooLarge);

        if (!AllowedContentTypes.Contains(contentType))
            return Result.Failure(FileStorageErrors.InvalidContentType);

        var ext = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(ext))
            return Result.Failure(FileStorageErrors.InvalidExtension);

        return Result.Success();
    }

    private async Task<Result<string>> SaveAsync(
        Stream stream,
        string originalFileName,
        CancellationToken cancellationToken)
    {
        try
        {
            var ext = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid():N}{ext}";

            var now = DateTime.UtcNow;
            var subFolder = Path.Combine("uploads", now.Year.ToString(), now.Month.ToString("D2"));
            var physicalDir = Path.Combine(_webRootPath, subFolder);

            Directory.CreateDirectory(physicalDir);

            var physicalPath = Path.Combine(physicalDir, uniqueFileName);

            await using var fileStream = new FileStream(
                physicalPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true);

            await stream.CopyToAsync(fileStream, cancellationToken);

            var relativeUrl = $"/{subFolder.Replace(Path.DirectorySeparatorChar, '/')}/{uniqueFileName}";

            _logger.LogInformation("Uploaded file: {RelativeUrl} ({Bytes} bytes)", relativeUrl, stream.Position);

            return Result<string>.Success(relativeUrl);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file: {OriginalFileName}", originalFileName);
            return Result<string>.Failure(FileStorageErrors.UploadFailed);
        }
    }
}