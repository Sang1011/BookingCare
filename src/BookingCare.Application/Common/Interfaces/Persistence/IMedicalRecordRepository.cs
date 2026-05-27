using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Domain.Entities.Booking;

namespace BookingCare.Application.Common.Interfaces.Persistence
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord>
    {
        Task<MedicalRecord?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);

        Task<MedicalRecordDto?> GetDtoByIdAsync(Guid id, CancellationToken ct = default);

        Task<MedicalRecordDto?> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);

        Task<(IReadOnlyList<MedicalRecordDto> Items, int TotalCount)> GetPagedByPatientAsync(
            Guid patientId,
            int page,
            int pageSize,
            CancellationToken ct = default);

        Task<bool> ExistsForBookingAsync(Guid bookingId, CancellationToken ct = default);
    }
}