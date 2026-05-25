using BookingCare.Domain.Entities.Doctor;

namespace BookingCare.Application.Common.Interfaces.Persistence
{
    public interface ISpecialtyRepository : IRepository<Specialty>
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Specialty>> GetAllAsync(CancellationToken ct = default);
    }
}