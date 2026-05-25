using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context) => _context = context;

        public async Task<Result> SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {
                await _context.SaveChangesAsync(ct);
                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                return Result.Failure(new Error("Database.SaveFailed", ex.Message));
            }
        }
    }
}
