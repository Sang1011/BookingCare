using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.MedicalRecords.DTOs
{
    public record MedicalRecordAttachmentDto(
        Guid Id,
        string FileName,
        string FileUrl,
        long FileSize,
        string ContentType,
        DateTime UploadedAt
    );
}
