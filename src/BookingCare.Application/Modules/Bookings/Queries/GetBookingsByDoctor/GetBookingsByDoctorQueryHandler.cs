using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.Queries.GetBookingsByDoctor;

public class GetBookingsByDoctorQueryHandler(
    IBookingRepository bookingRepository,
    ICurrentUser currentUser
) : IRequestHandler<GetBookingsByDoctorQuery, Result<PagedResult<BookingDto>>>
{
    public async Task<Result<PagedResult<BookingDto>>> Handle(
        GetBookingsByDoctorQuery request,
        CancellationToken ct)
    {
        if (currentUser.Role == UserRole.Doctor && currentUser.UserId != request.DoctorId)
            return Result<PagedResult<BookingDto>>.Failure(BookingErrors.Unauthorized);

        if (currentUser.Role == UserRole.Patient)
            return Result<PagedResult<BookingDto>>.Failure(BookingErrors.Unauthorized);

        var (items, total) = await bookingRepository.GetPagedByDoctorAsync(
            request.DoctorId,
            request.Page,
            request.PageSize,
            request.StatusFilter,
            request.DateFilter,
            ct);

        return Result<PagedResult<BookingDto>>.Success(
            new PagedResult<BookingDto>(items, total, request.Page, request.PageSize));
    }
}