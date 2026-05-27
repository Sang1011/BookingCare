using BookingCare.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCare.Infrastructure.Persistence.Configurations
{
    public class MedicalRecordAttachmentConfiguration : IEntityTypeConfiguration<MedicalRecordAttachment>
    {
        public void Configure(EntityTypeBuilder<MedicalRecordAttachment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.MedicalRecordId)
                .IsRequired();

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FileUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(a => a.FileSize)
                .IsRequired();

            builder.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.UploadedAt)
                .IsRequired();

            builder.HasOne(a => a.MedicalRecord)
                .WithMany(m => m.Attachments)
                .HasForeignKey(a => a.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.MedicalRecordId);
        }
    }
}