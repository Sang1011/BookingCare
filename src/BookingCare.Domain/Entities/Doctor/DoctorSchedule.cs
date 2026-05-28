using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using BookingCare.Domain.Events;
using BookingCare.Domain.ValueObjects;

namespace BookingCare.Domain.Entities.Doctor;

public class DoctorSchedule : AuditableEntity
{
    public Guid DoctorId { get; private set; }
    public TimeSlot TimeSlot { get; private set; } = default!;
    public int MaxPatients { get; private set; }
    public bool IsAvailable { get; private set; }
    public uint xmin { get; private set; }
    public Doctor Doctor { get; private set; } = null!;
    public DateOnly WorkDate => TimeSlot.WorkDate;
    public TimeOnly SlotStart => TimeSlot.SlotStart;
    public TimeOnly SlotEnd => TimeSlot.SlotEnd;

    private DoctorSchedule() { }

    public static Result<DoctorSchedule> Create(
        Guid doctorId,
        string doctorName,
        DateOnly workDate,
        TimeOnly slotStart,
        TimeOnly slotEnd,
        int maxPatients = 1)
    {
        if (doctorId == Guid.Empty)
            return Result<DoctorSchedule>.Failure(DoctorScheduleErrors.InvalidDoctorId);

        if (maxPatients < 1)
            return Result<DoctorSchedule>.Failure(DoctorScheduleErrors.InvalidMaxPatients);

        var slotResult = TimeSlot.Create(workDate, slotStart, slotEnd);
        if (slotResult.IsFailure)
            return Result<DoctorSchedule>.Failure(slotResult.Error!);

        var schedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            TimeSlot = slotResult.Value!,
            MaxPatients = maxPatients,
            IsAvailable = true
        };

        schedule.Touch();
        schedule.RaiseDomainEvent(new DoctorScheduleCreatedEvent(schedule.Id, doctorId, doctorName, workDate, slotStart, slotEnd));
        return Result<DoctorSchedule>.Success(schedule);
    }
    
    public bool IsExpired() => TimeSlot.IsExpired();

    public void MarkUnavailable() => IsAvailable = false;
    public void MarkAvailable() => IsAvailable = true;
}