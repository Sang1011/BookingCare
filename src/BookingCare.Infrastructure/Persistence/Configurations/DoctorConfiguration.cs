using BookingCare.Domain.Entities.Doctor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCare.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);

        builder.HasOne(d => d.User)
            .WithOne()
            .HasForeignKey<Doctor>(d => d.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Specialty)
            .WithMany()
            .HasForeignKey(d => d.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(d => d.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(d => d.LicenseNumber)
            .IsUnique();

        builder.Property(d => d.ConsultationFee)
            .HasColumnType("numeric(18,2)");

        builder.Property(d => d.Bio)
            .HasMaxLength(1000);

        builder.Property(d => d.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(d => d.IsVerified)
            .IsRequired()
            .HasDefaultValue(false);
    }
}