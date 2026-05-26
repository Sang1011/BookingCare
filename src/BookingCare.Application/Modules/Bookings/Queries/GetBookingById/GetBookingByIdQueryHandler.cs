using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Bookings.Queries.GetBookingById;

public class GetBookingByIdQueryHandler(
    IBookingRepository bookingRepository,
    ICurrentUser currentUser
) : IRequestHandler<GetBookingByIdQuery, Result<BookingDetailDto>>
{
    public async Task<Result<BookingDetailDto>> Handle(GetBookingByIdQuery request, CancellationToken ct)
    {
        var detail = await bookingRepository.GetDetailByIdAsync(request.BookingId, ct);
        if (detail is null)
            return Result<BookingDetailDto>.Failure(BookingErrors.NotFound);

        // Patient chỉ xem được booking của mình
        if (currentUser.Role == UserRole.Patient && detail.PatientId != currentUser.UserId)
            return Result<BookingDetailDto>.Failure(BookingErrors.Unauthorized);

        // Doctor chỉ xem được booking thuộc lịch của mình
        if (currentUser.Role == UserRole.Doctor && detail.DoctorId != currentUser.UserId)
            return Result<BookingDetailDto>.Failure(BookingErrors.Unauthorized);

        return Result<BookingDetailDto>.Success(detail);
    }
}