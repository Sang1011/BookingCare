using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.Queries.GetDoctorById
{
    public record GetDoctorByIdQuery(Guid DoctorId) : IRequest<Result<DoctorDto>>;
}
