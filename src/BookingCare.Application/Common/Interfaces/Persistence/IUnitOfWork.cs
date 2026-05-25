using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        Task<Result> SaveChangesAsync(CancellationToken ct = default);
    }
}
