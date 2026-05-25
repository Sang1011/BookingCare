using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.DTOs
{
    public record DoctorDto(
        Guid Id,
        string FullName,
        string Email,
        string SpecialtyName,
        string LicenseNumber,
        int YearsOfExperience,
        decimal ConsultationFee,
        string? Bio,
        string? AvatarUrl,
        bool IsVerified
    );
}
