using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Queries.GetBookings;

public record GetBookingsQuery(
    int Page = 1,
    int PageSize = 20,
    BookingStatus? StatusFilter = null
) : IRequest<Result<PagedResult<BookingDto>>>;