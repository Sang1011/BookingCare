using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Domain.Entities.Booking;
using BookingCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Infrastructure.Persistence.Repositories;

public class BookingRepository : BaseRepository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context) { }

    public async Task<bool> HasConfirmedBookingAsync(
        Guid doctorScheduleId,
        CancellationToken ct = default)
        => await _context.Bookings.AnyAsync(b =>
            b.DoctorScheduleId == doctorScheduleId &&
            b.Status == BookingStatus.Confirmed, ct);

    public async Task<bool> HasActiveBookingWithDoctorAsync(
        Guid patientId,
        Guid doctorId,
        CancellationToken ct = default)
        => await _context.Bookings.AnyAsync(b =>
            b.PatientId == patientId &&
            b.DoctorSchedule.DoctorId == doctorId &&
            (b.Status == BookingStatus.Pending ||
             b.Status == BookingStatus.Confirmed), ct);

    public async Task<Booking?> GetByIdWithScheduleAsync(
        Guid bookingId,
        CancellationToken ct = default)
        => await _context.Bookings
            .Include(b => b.DoctorSchedule)
                .ThenInclude(s => s.Doctor)
            .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

    public async Task<BookingDetailDto?> GetDetailByIdAsync(
        Guid bookingId,
        CancellationToken ct = default)
        => await _context.Bookings
            .Where(b => b.Id == bookingId)
            .Select(b => new BookingDetailDto(
                b.Id,
                b.PatientId,
                b.Patient.FullName,                       
                b.Patient.Email,                          
                b.Patient.Phone ?? string.Empty,          
                b.DoctorSchedule.DoctorId,
                b.DoctorSchedule.Doctor.User.FullName,   
                b.DoctorSchedule.Doctor.Specialty.Name,
                b.DoctorSchedule.Doctor.ConsultationFee,
                b.DoctorSchedule.WorkDate,
                b.DoctorSchedule.SlotStart,
                b.DoctorSchedule.SlotEnd,
                b.Status,
                b.Notes,
                b.CancellationReason,
                b.CancelledBy.ToString(),
                b.RescheduledFromId,
                b.CreatedAt,
                b.UpdatedAt
            ))
            .FirstOrDefaultAsync(ct);

    public async Task<(List<BookingDto> Items, int TotalCount)> GetPagedByPatientAsync(
        Guid patientId,
        int page,
        int pageSize,
        BookingStatus? statusFilter,
        CancellationToken ct = default)
    {
        var query = _context.Bookings
            .Where(b => b.PatientId == patientId);

        if (statusFilter.HasValue)
            query = query.Where(b => b.Status == statusFilter.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookingDto(
                b.Id,
                b.PatientId,
                b.Patient.FullName,                    
                b.DoctorSchedule.DoctorId,
                b.DoctorSchedule.Doctor.User.FullName, 
                b.DoctorSchedule.Doctor.Specialty.Name,
                b.DoctorSchedule.WorkDate,
                b.DoctorSchedule.SlotStart,
                b.DoctorSchedule.SlotEnd,
                b.Status,
                b.Notes,
                b.CreatedAt
            ))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<(List<BookingDto> Items, int TotalCount)> GetPagedByDoctorAsync(
        Guid doctorId,
        int page,
        int pageSize,
        BookingStatus? statusFilter,
        DateOnly? dateFilter,
        CancellationToken ct = default)
    {
        var query = _context.Bookings
            .Where(b => b.DoctorSchedule.DoctorId == doctorId);

        if (statusFilter.HasValue)
            query = query.Where(b => b.Status == statusFilter.Value);

        if (dateFilter.HasValue)
            query = query.Where(b => b.DoctorSchedule.WorkDate == dateFilter.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(b => b.DoctorSchedule.WorkDate)
            .ThenBy(b => b.DoctorSchedule.SlotStart)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookingDto(
                b.Id,
                b.PatientId,
                b.Patient.FullName,                     
                b.DoctorSchedule.DoctorId,
                b.DoctorSchedule.Doctor.User.FullName,
                b.DoctorSchedule.Doctor.Specialty.Name,
                b.DoctorSchedule.WorkDate,
                b.DoctorSchedule.SlotStart,
                b.DoctorSchedule.SlotEnd,
                b.Status,
                b.Notes,
                b.CreatedAt
            ))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<List<Booking>> GetExpiredPendingBookingsAsync(
        CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);

        return await _context.Bookings
            .Include(b => b.DoctorSchedule)
            .Where(b =>
                b.Status == BookingStatus.Pending &&
                b.CreatedAt <= cutoff)
            .ToListAsync(ct);
    }
}