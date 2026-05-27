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

            builder.OwnsOne(d => d.TimeSlot, ts =>
            {
                ts.Property(t => t.WorkDate)
                  .HasColumnName("work_date")
                  .IsRequired();

                ts.Property(t => t.SlotStart)
                  .HasColumnName("slot_start")
                  .IsRequired();

                ts.Property(t => t.SlotEnd)
                  .HasColumnName("slot_end")
                  .IsRequired();
            });

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
