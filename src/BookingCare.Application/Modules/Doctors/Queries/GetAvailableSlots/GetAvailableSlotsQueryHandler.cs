using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.Queries.GetAvailableSlots
{
    public class GetAvailableSlotsQueryHandler(
        IDoctorRepository doctorRepository,
        IDoctorScheduleRepository scheduleRepository)
        : IRequestHandler<GetAvailableSlotsQuery, Result<IReadOnlyList<AvailableSlotDto>>>
    {
        public async Task<Result<IReadOnlyList<AvailableSlotDto>>> Handle(
            GetAvailableSlotsQuery request,
            CancellationToken cancellationToken)
        {
            var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
            if (doctor is null)
                return Result<IReadOnlyList<AvailableSlotDto>>.Failure(DoctorErrors.NotFound);

            var slots = await scheduleRepository.GetAvailableSlotsByDateAsync(
                request.DoctorId, request.Date, cancellationToken);

            var dtos = slots
                .Where(s => !s.IsExpired())
                .Select(s => new AvailableSlotDto(
                    s.Id,
                    s.SlotStart,
                    s.SlotEnd,
                    doctor.ConsultationFee))
                .ToList();

            return Result<IReadOnlyList<AvailableSlotDto>>.Success(dtos);
        }
    }
}
