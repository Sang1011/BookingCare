using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.Queries.GetDoctors
{
    public record GetDoctorsQuery(
        string? SearchName,
        Guid? SpecialtyId,
        DateOnly? AvailableDate,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<Result<PagedResult<DoctorDto>>>;

    public record PagedResult<T>(
        IReadOnlyList<T> Items,
        int TotalCount,
        int Page,
        int PageSize)
    {
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    };
}
