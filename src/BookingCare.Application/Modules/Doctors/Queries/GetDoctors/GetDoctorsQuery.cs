using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Doctors.Queries.GetDoctors;

public record GetDoctorsQuery(
    string? SearchName,
    Guid? SpecialtyId,
    DateOnly? AvailableDate,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<DoctorDto>>>;