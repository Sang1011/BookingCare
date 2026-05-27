using BookingCare.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCare.Infrastructure.Persistence.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.BookingId)
                .IsRequired();

            builder.Property(m => m.PatientId)
                .IsRequired();

            builder.Property(m => m.DoctorId)
                .IsRequired();

            builder.Property(m => m.Diagnosis)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(m => m.Treatment)
                .HasMaxLength(2000);

            builder.Property(m => m.Notes)
                .HasMaxLength(1000);

            builder.Property(m => m.VisitDate)
                .IsRequired();

            builder.Property(m => m.FollowUpDate)
                .IsRequired(false);

            builder.HasOne(m => m.Booking)
                .WithOne()
                .HasForeignKey<MedicalRecord>(m => m.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => m.BookingId)
                .IsUnique();

            builder.HasIndex(m => m.PatientId);

            builder.HasIndex(m => m.DoctorId);

            builder.HasIndex(m => m.VisitDate);
        }
    }
}