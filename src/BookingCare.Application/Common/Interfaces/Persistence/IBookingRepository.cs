using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Domain.Entities.Booking;
using BookingCare.Domain.Enums;

namespace BookingCare.Application.Common.Interfaces.Persistence;

public interface IBookingRepository : IRepository<Booking>
{
    Task<bool> HasConfirmedBookingAsync(Guid doctorScheduleId, CancellationToken ct = default);
    Task<bool> HasActiveBookingWithDoctorAsync(Guid patientId, Guid doctorId, CancellationToken ct = default);
    Task<Booking?> GetByIdWithScheduleAsync(Guid bookingId, CancellationToken ct = default);
    Task<(List<BookingDto> Items, int TotalCount)> GetPagedByPatientAsync(
        Guid patientId,
        int page,
        int pageSize,
        BookingStatus? statusFilter,
        CancellationToken ct = default);
    Task<(List<BookingDto> Items, int TotalCount)> GetPagedByDoctorAsync(
        Guid doctorId,
        int page,
        int pageSize,
        BookingStatus? statusFilter,
        DateOnly? dateFilter,
        CancellationToken ct = default);
    Task<BookingDetailDto?> GetDetailByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<List<Booking>> GetExpiredPendingBookingsAsync(CancellationToken ct = default);
}