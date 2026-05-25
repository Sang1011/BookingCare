using BookingCare.Application.Modules.Auth.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Auth.Commands.RegisterDoctor
{
    public record RegisterDoctorCommand(
        string Email,
        string Password,
        string FullName,
        string? Phone,
        DateOnly? DateOfBirth,
        string LicenseNumber,
        Guid SpecialtyId,
        int YearsOfExperience,
        decimal ConsultationFee,
        string? Bio
    ) : IRequest<Result<UserDto>>;
}
