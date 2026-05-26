using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Queries.GetBookingsByDoctor;

public sealed record GetBookingsByDoctorQuery(
    Guid DoctorId,
    int Page = 1,
    int PageSize = 20,
    BookingStatus? StatusFilter = null,
    DateOnly? DateFilter = null
) : IRequest<Result<PagedResult<BookingDto>>>;