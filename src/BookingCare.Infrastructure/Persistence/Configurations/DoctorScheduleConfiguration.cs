using BookingCare.Domain.Entities.Doctor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCare.Infrastructure.Persistence.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.xmin)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

            builder.HasOne(d => d.Doctor)
            .WithMany(d => d.Schedules)
            .HasForeignKey(d => d.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
