using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Doctors.Commands.DeleteSchedule
{
    public class DeleteScheduleCommandHandler(
        IDoctorScheduleRepository scheduleRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteScheduleCommand, Result>
    {
        public async Task<Result> Handle(
            DeleteScheduleCommand request,
            CancellationToken cancellationToken)
        {
            var schedule = await scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule is null)
                return Result.Failure(DoctorScheduleErrors.NotFound);

            if (currentUser.Role != UserRole.Admin && schedule.DoctorId != currentUser.UserId)
                return Result.Failure(CommonErrors.Unauthorized);

            var hasBookings = await scheduleRepository.HasActiveBookingsAsync(
                request.ScheduleId, cancellationToken);

            if (hasBookings)
                return Result.Failure(DoctorScheduleErrors.HasActiveBookings);

            scheduleRepository.Remove(schedule);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
