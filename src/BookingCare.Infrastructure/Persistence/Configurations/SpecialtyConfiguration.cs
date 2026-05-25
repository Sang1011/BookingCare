using BookingCare.Domain.Entities.Doctor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCare.Infrastructure.Persistence.Configurations
{
    public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
    {
        public void Configure(EntityTypeBuilder<Specialty> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => s.Name)
                .IsUnique();

            builder.Property(s => s.Description)
                .HasMaxLength(500);
        }
    }
}