using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookingCare.Infrastructure.Storage;

public class AzureBlobStorageService : IFileStorageService
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
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(
        IOptions<AzureBlobStorageSettings> options,
        ILogger<AzureBlobStorageService> logger)
    {
        var settings = options.Value;
        _logger = logger;

        var serviceClient = new BlobServiceClient(settings.ConnectionString);
        _containerClient = serviceClient.GetBlobContainerClient(settings.ContainerName);
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

        return await UploadToBlobAsync(stream, fileName, contentType, cancellationToken);
    }

    public async Task<Result> DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var blobName = ExtractBlobName(fileUrl);
            if (blobName is null)
            {
                _logger.LogWarning("Cannot extract blob name from URL: {FileUrl}", fileUrl);
                return Result.Failure(FileStorageErrors.InvalidPath);
            }

            var blobClient = _containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                cancellationToken: cancellationToken);

            if (!response.Value)
            {
                _logger.LogWarning("Blob not found for deletion: {BlobName}", blobName);
                return Result.Failure(FileStorageErrors.FileNotFound);
            }

            _logger.LogInformation("Deleted blob: {BlobName}", blobName);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure request failed while deleting: {FileUrl}", fileUrl);
            return Result.Failure(FileStorageErrors.DeleteFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete blob: {FileUrl}", fileUrl);
            return Result.Failure(FileStorageErrors.DeleteFailed);
        }
    }

    private async Task<Result<string>> UploadToBlobAsync(
        Stream stream,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        try
        {
            await _containerClient.CreateIfNotExistsAsync(
                PublicAccessType.Blob,
                cancellationToken: cancellationToken);

            var ext = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid():N}{ext}";
            var now = DateTime.UtcNow;
            var blobName = $"uploads/{now.Year}/{now.Month:D2}/{uniqueFileName}";

            var blobClient = _containerClient.GetBlobClient(blobName);

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            };

            await blobClient.UploadAsync(stream, uploadOptions, cancellationToken);

            var fileUrl = blobClient.Uri.ToString();

            _logger.LogInformation("Uploaded blob: {BlobName} ({Bytes} bytes)", blobName, stream.Position);

            return Result<string>.Success(fileUrl);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure request failed while uploading: {FileName}", originalFileName);
            return Result<string>.Failure(FileStorageErrors.UploadFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload blob: {FileName}", originalFileName);
            return Result<string>.Failure(FileStorageErrors.UploadFailed);
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

    private string? ExtractBlobName(string fileUrl)
    {
        if (!Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
            return null;

        var containerPrefix = $"/{_containerClient.Name}/";
        var path = uri.AbsolutePath;

        return path.StartsWith(containerPrefix, StringComparison.OrdinalIgnoreCase)
            ? path[containerPrefix.Length..]
            : null;
    }
}