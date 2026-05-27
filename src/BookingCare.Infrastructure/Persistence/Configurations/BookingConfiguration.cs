using BookingCare.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCare.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.PatientId)
            .IsRequired();

        builder.Property(b => b.DoctorScheduleId)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.Property(b => b.CancellationReason)
            .HasMaxLength(500);

        builder.Property(b => b.CancelledBy)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(b => b.RescheduledFromId)
            .IsRequired(false);

        builder.Property(b => b.RescheduleCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.DoctorRescheduleCount)
            .IsRequired()
            .HasDefaultValue(0);

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

        builder.HasIndex(b => b.PatientId);

        builder.HasIndex(b => b.DoctorScheduleId);

        builder.HasIndex(b => b.Status);

        builder.HasIndex(b => b.RescheduledFromId);
    }
}