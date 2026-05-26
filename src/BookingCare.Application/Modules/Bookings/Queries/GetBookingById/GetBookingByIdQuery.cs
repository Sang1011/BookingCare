using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Bookings.Queries.GetBookingById;

public record GetBookingByIdQuery(Guid BookingId) : IRequest<Result<BookingDetailDto>>;