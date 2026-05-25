using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Entities.Doctor;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Persistence
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<Doctor?> GetByIdWithSpecialtyAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid userId, CancellationToken ct = default);
        Task<bool> LicenseNumberExistsAsync(string licenseNumber, CancellationToken ct = default);

        Task<(IReadOnlyList<DoctorDto> Items, int TotalCount)> GetPagedAsync(
            string? searchName,
            Guid? specialtyId,
            DateOnly? availableDate,
            int page,
            int pageSize,
            CancellationToken ct = default);
    }
}
