using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Entities.Doctor
{
    public class Doctor : BaseEntity
    {
        public string LicenseNumber { get; private set; } = default!;
        public Guid SpecialtyId { get; private set; }
        public int YearsOfExperience { get; private set; }
        public string? Bio { get; private set; }
        public decimal ConsultationFee { get; private set; }
        public string? AvatarUrl { get; private set; }

        // Navigation
        public Specialty Specialty { get; private set; } = default!;
        public ICollection<DoctorSchedule> Schedules { get; private set; } = [];

        private Doctor() { }

        public static Doctor Create(
            Guid userId, Guid specialtyId, string licenseNumber,
            int yearsOfExperience, decimal consultationFee, string? bio)
        {
            return new Doctor
            {
                Id = userId, // Doctor ID = User ID
                SpecialtyId = specialtyId,
                LicenseNumber = licenseNumber,
                YearsOfExperience = yearsOfExperience,
                ConsultationFee = consultationFee,
                Bio = bio
            };
        }
    }
}
