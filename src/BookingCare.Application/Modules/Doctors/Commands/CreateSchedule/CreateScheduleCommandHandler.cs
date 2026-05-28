using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Doctor;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Doctors.Commands.CreateSchedule
{
    public class CreateScheduleCommandHandler(
        IDoctorRepository doctorRepository,
        IDoctorScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateScheduleCommand, Result<DoctorScheduleDto>>
    {
        public async Task<Result<DoctorScheduleDto>> Handle(
            CreateScheduleCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
            if (doctor is null)
                return Result<DoctorScheduleDto>.Failure(DoctorErrors.NotFound);

            if (!doctor.IsVerified)
                return Result<DoctorScheduleDto>.Failure(DoctorErrors.NotVerified);

            var hasConflict = await scheduleRepository.HasConflictAsync(
                request.DoctorId,
                request.WorkDate,
                request.SlotStart,
                request.SlotEnd,
                cancellationToken);

            if (hasConflict)
                return Result<DoctorScheduleDto>.Failure(DoctorScheduleErrors.SlotConflict);

            var result = DoctorSchedule.Create(
                request.DoctorId,
                doctor.User.FullName,
                request.WorkDate,
                request.SlotStart,
                request.SlotEnd,
                request.MaxPatients);

            if (result.IsFailure)
                return Result<DoctorScheduleDto>.Failure(result.Error!);

            var schedule = result.Value!; 

            scheduleRepository.Add(schedule);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new DoctorScheduleDto(
                schedule.Id,
                schedule.WorkDate,
                schedule.SlotStart,
                schedule.SlotEnd,
                schedule.MaxPatients,
                schedule.IsAvailable);

            return Result<DoctorScheduleDto>.Success(dto);
        }
    }
}
