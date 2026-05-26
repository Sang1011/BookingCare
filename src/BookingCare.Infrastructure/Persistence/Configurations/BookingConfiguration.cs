using BookingCare.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCare.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.CancelledBy)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.Property(b => b.CancellationReason)
            .HasMaxLength(500);

        builder.HasOne(b => b.DoctorSchedule)
            .WithMany()
            .HasForeignKey(b => b.DoctorScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Patient)
            .WithMany()
            .HasForeignKey(b => b.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Booking>()
            .WithMany()
            .HasForeignKey(b => b.RescheduledFromId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }
}