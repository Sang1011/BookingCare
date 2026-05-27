using BookingCare.Application.Common.Models;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Auth;
using BookingCare.Domain.Entities.Booking;
using BookingCare.Domain.Entities.Doctor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IPublisher _publisher;

        public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher)
            : base(options)
        {
            _publisher = publisher;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Specialty> Specialties => Set<Specialty>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
        public DbSet<MedicalRecordAttachment> MedicalRecordAttachments => Set<MedicalRecordAttachment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var entitiesWithEvents = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            var result = await base.SaveChangesAsync(ct);

            foreach (var domainEvent in domainEvents)
            {
                var notification = CreateNotification(domainEvent);
                await _publisher.Publish(notification, ct);
            }

            return result;
        }

        private static object CreateNotification(IDomainEvent domainEvent)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());
            return Activator.CreateInstance(notificationType, domainEvent)!;
        }
    }
}