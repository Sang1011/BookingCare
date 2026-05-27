using BookingCare.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.OwnsOne(u => u.Email, e =>
            {
                e.Property(x => x.Value)
                 .HasColumnName("email")
                 .HasMaxLength(256)
                 .IsRequired();

                e.HasIndex(x => x.Value).IsUnique();
            });

            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Phone).HasMaxLength(11);
            builder.Property(u => u.Role).HasConversion<string>();
        }
    }
}
