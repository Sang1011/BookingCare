using BookingCare.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCare.Infrastructure.Persistence.Configurations
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.MedicalRecordId)
                .IsRequired();

            builder.Property(p => p.MedicineName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Dosage)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Frequency)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Duration)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Instructions)
                .HasMaxLength(500);

            builder.HasOne(p => p.MedicalRecord)
                .WithMany(m => m.PrescriptionItems)
                .HasForeignKey(p => p.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.MedicalRecordId);
        }
    }
}