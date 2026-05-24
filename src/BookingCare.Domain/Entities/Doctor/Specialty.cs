using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Entities.Doctor
{
    public class Specialty : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Specialty() { }

        public static Specialty Create(string name, string? description) =>
            new() { Name = name, Description = description };
    }
}
