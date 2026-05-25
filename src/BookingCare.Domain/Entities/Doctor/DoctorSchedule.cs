using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using BookingCare.Domain.Events;

namespace BookingCare.Domain.Entities.Doctor;

public class DoctorSchedule : AuditableEntity
{
    public Guid DoctorId { get; private set; }
    public DateOnly WorkDate { get; private set; }
    public TimeOnly SlotStart { get; private set; }
    public TimeOnly SlotEnd { get; private set; }
    public int MaxPatients { get; private set; }
    public bool IsAvailable { get; private set; }
    public uint RowVersion { get; private set; }
    public Doctor Doctor { get; private set; } = null!;

    private DoctorSchedule() { }

    public static Result<DoctorSchedule> Create(
        Guid doctorId,
        DateOnly workDate,
        TimeOnly slotStart,
        TimeOnly slotEnd,
        int maxPatients = 1)
    {
        if (doctorId == Guid.Empty)
            return Result<DoctorSchedule>.Failure(DoctorScheduleErrors.InvalidDoctorId);

        if (workDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            return Result<DoctorSchedule>.Failure(DoctorScheduleErrors.WorkDateInPast);

        if (slotEnd <= slotStart)
            return Result<DoctorSchedule>.Failure(DoctorScheduleErrors.InvalidSlotRange);

        if (maxPatients < 1)
            return Result<DoctorSchedule>.Failure(DoctorScheduleErrors.InvalidMaxPatients);

        var workStart = new TimeOnly(7, 0);
        var workEnd = new TimeOnly(17, 0);

        if (slotStart < workStart || slotEnd > workEnd)
            return Result<DoctorSchedule>.Failure(DoctorScheduleErrors.SlotOutsideWorkingHours);

        var schedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            WorkDate = workDate,
            SlotStart = slotStart,
            SlotEnd = slotEnd,
            MaxPatients = maxPatients,
            IsAvailable = true
        };

        schedule.Touch();
        schedule.RaiseDomainEvent(new DoctorScheduleCreatedEvent(schedule.Id, doctorId, workDate, slotStart));
        return Result<DoctorSchedule>.Success(schedule);
    }

    public bool IsExpired() =>
        WorkDate < DateOnly.FromDateTime(DateTime.UtcNow.Date) ||
        (WorkDate == DateOnly.FromDateTime(DateTime.UtcNow.Date) &&
         SlotStart < TimeOnly.FromDateTime(DateTime.UtcNow));

    public void MarkUnavailable() => IsAvailable = false;
    public void MarkAvailable() => IsAvailable = true;
}