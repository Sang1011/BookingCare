using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.DTOs
{
    public record SpecialtyDto(
        Guid Id,
        string Name,
        string? Description
    );
}
