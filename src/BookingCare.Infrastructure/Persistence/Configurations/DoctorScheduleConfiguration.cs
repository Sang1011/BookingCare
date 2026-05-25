using BookingCare.Domain.Entities.Doctor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Persistence.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.RowVersion)
                .IsRowVersion();

            builder.HasOne<Doctor>()
                .WithMany(d => d.Schedules)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
