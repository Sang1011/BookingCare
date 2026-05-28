namespace BookingCare.Infrastructure.Storage;

public class AzureBlobStorageSettings
{
    public const string SectionName = "AzureBlobStorage";

    /// <summary>
    /// Connection string từ Azure Portal → Storage Account → Access keys
    /// Ví dụ: "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net"
    /// Nên lưu trong Azure Key Vault hoặc Secret Manager, không commit vào git.
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Tên container trong Blob Storage (lowercase, chỉ chứa chữ thường, số, dấu gạch ngang)
    /// Ví dụ: "bookingcare-attachments"
    /// </summary>
    public string ContainerName { get; init; } = string.Empty;
}