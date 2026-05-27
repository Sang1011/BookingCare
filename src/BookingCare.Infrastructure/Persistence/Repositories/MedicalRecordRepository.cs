using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Infrastructure.Persistence.Repositories
{
    public class MedicalRecordRepository(AppDbContext context)
        : BaseRepository<MedicalRecord>(context), IMedicalRecordRepository
    {
        public async Task<MedicalRecordDto?> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
            => await Project().FirstOrDefaultAsync(m => m.BookingId == bookingId, ct);

        public async Task<MedicalRecord?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
            => await _dbSet
                .Include(m => m.PrescriptionItems)
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == id, ct);

        public async Task<bool> ExistsForBookingAsync(Guid bookingId, CancellationToken ct = default)
            => await _dbSet.AnyAsync(m => m.BookingId == bookingId, ct);

        public async Task<(IReadOnlyList<MedicalRecordDto> Items, int TotalCount)> GetPagedByPatientAsync(
            Guid patientId,
            int page,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = Project().Where(m => m.PatientId == patientId);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(m => m.VisitDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<MedicalRecordDto?> GetDtoByIdAsync(Guid id, CancellationToken ct = default)
            => await Project().FirstOrDefaultAsync(m => m.Id == id, ct);

        private IQueryable<MedicalRecordDto> Project()
            => _dbSet
                .AsNoTracking()
                .Select(m => new MedicalRecordDto(
                    m.Id,
                    m.BookingId,
                    m.PatientId,
                    m.Booking.Patient.FullName,
                    m.DoctorId,
                    m.Booking.DoctorSchedule.Doctor.User.FullName,
                    m.Booking.DoctorSchedule.Doctor.Specialty.Name,
                    m.VisitDate,
                    m.Diagnosis,
                    m.Treatment,
                    m.Notes,
                    m.FollowUpDate,
                    m.PrescriptionItems
                        .Select(p => new PrescriptionItemDto(
                            p.Id,
                            p.MedicineName,
                            p.Dosage,
                            p.Frequency,
                            p.Duration,
                            p.Instructions))
                        .ToList(),
                    m.Attachments
                        .Select(a => new MedicalRecordAttachmentDto(
                            a.Id,
                            a.FileName,
                            a.FileUrl,
                            a.FileSize,
                            a.ContentType,
                            a.UploadedAt))
                        .ToList(),
                    m.CreatedAt,
                    m.UpdatedAt
                ));
    }
}