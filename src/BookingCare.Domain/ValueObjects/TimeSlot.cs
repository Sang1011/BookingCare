using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;

namespace BookingCare.Domain.ValueObjects;

public sealed record TimeSlot
{
    public DateOnly WorkDate { get; }
    public TimeOnly SlotStart { get; }
    public TimeOnly SlotEnd { get; }

    private static readonly TimeOnly WorkDayStart = new(7, 0);
    private static readonly TimeOnly WorkDayEnd = new(17, 0);

    private TimeSlot(DateOnly workDate, TimeOnly slotStart, TimeOnly slotEnd)
    {
        WorkDate = workDate;
        SlotStart = slotStart;
        SlotEnd = slotEnd;
    }

    public static Result<TimeSlot> Create(DateOnly workDate, TimeOnly slotStart, TimeOnly slotEnd)
    {
        if (workDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            return Result<TimeSlot>.Failure(DoctorScheduleErrors.WorkDateInPast);

        if (slotEnd <= slotStart)
            return Result<TimeSlot>.Failure(DoctorScheduleErrors.InvalidSlotRange);

        if (slotStart < WorkDayStart || slotEnd > WorkDayEnd)
            return Result<TimeSlot>.Failure(DoctorScheduleErrors.SlotOutsideWorkingHours);

        return Result<TimeSlot>.Success(new TimeSlot(workDate, slotStart, slotEnd));
    }

    public bool IsExpired()
    {
        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);
        return WorkDate < today ||
               (WorkDate == today && SlotStart < TimeOnly.FromDateTime(now));
    }

    public DateTime ToStartDateTime() => WorkDate.ToDateTime(SlotStart);

    public bool OverlapsWith(TimeSlot other)
        => WorkDate == other.WorkDate
        && SlotStart < other.SlotEnd
        && other.SlotStart < SlotEnd;
}