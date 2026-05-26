using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Queries.GetBookings;

public class GetBookingsQueryHandler(
    IBookingRepository bookingRepository,
    ICurrentUser currentUser
) : IRequestHandler<GetBookingsQuery, Result<PagedResult<BookingDto>>>
{
    public async Task<Result<PagedResult<BookingDto>>> Handle(
        GetBookingsQuery request,
        CancellationToken ct)
    {
        (List<BookingDto> items, int total) = currentUser.Role == UserRole.Doctor
            ? await bookingRepository.GetPagedByDoctorAsync(
                currentUser.UserId, request.Page, request.PageSize, request.StatusFilter, null, ct)
            : await bookingRepository.GetPagedByPatientAsync(
                currentUser.UserId, request.Page, request.PageSize, request.StatusFilter, ct);

        return Result<PagedResult<BookingDto>>.Success(
            new PagedResult<BookingDto>(items, total, request.Page, request.PageSize));
    }
}