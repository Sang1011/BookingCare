using BookingCare.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Status)
                .HasConversion<string>();

            builder.Property(b => b.CancelledBy)
                .HasConversion<string>();

            builder.HasOne(b => b.DoctorSchedule)
                .WithMany()
                .HasForeignKey(b => b.DoctorScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
