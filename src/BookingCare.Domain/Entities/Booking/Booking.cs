using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Auth;
using BookingCare.Domain.Entities.Doctor;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using BookingCare.Domain.Events;

namespace BookingCare.Domain.Entities.Booking
{
    public class Booking : AuditableEntity
    {
        public Guid PatientId { get; private set; }
        public Guid DoctorScheduleId { get; private set; }
        public BookingStatus Status { get; private set; }
        public string? Notes { get; private set; }
        public string? CancellationReason { get; private set; }
        public CancelledBy? CancelledBy { get; private set; }
        public Guid? RescheduledFromId { get; private set; }
        public int RescheduleCount { get; private set; } = 0;
        public int DoctorRescheduleCount { get; private set; } = 0;

        public DoctorSchedule DoctorSchedule { get; private set; } = default!;
        public User Patient { get; private set; } = default!;

        private Booking() { }

        public static Booking Create(Guid patientId, Guid scheduleId, string? notes)
        {
            var booking = new Booking
            {
                PatientId = patientId,
                DoctorScheduleId = scheduleId,
                Status = BookingStatus.Pending,
                Notes = notes?.Trim()
            };

            booking.Touch();
            booking.RaiseDomainEvent(new BookingCreatedEvent(booking.Id, patientId, scheduleId));
            return booking;
        }

        public Result Confirm()
        {
            if (Status != BookingStatus.Pending)
                return Result.Failure(BookingErrors.CannotConfirmNonPending);

            Status = BookingStatus.Confirmed;
            Touch();
            RaiseDomainEvent(new BookingConfirmedEvent(Id, PatientId, DoctorScheduleId));
            return Result.Success();
        }

        public Result Cancel(string reason, CancelledBy cancelledBy)
        {
            if (Status is BookingStatus.Completed or BookingStatus.Cancelled)
                return Result.Failure(BookingErrors.AlreadyFinalized);

            if (cancelledBy == Enums.CancelledBy.Patient && Status == BookingStatus.Confirmed)
            {
                if (DoctorSchedule.TimeSlot.ToStartDateTime() - DateTime.UtcNow < TimeSpan.FromHours(2))
                    return Result.Failure(BookingErrors.CancellationWindowExpired);
            }

            if (cancelledBy == Enums.CancelledBy.Doctor && Status == BookingStatus.Confirmed)
            {
                var slotStart = DoctorSchedule.TimeSlot.ToStartDateTime();
                if (slotStart - DateTime.UtcNow < TimeSpan.FromHours(2))
                {
                    RaiseDomainEvent(new DoctorPenaltyEvent(
                        BookingId: Id,
                        DoctorId: DoctorSchedule.DoctorId,
                        PatientId: PatientId,
                        CancelledAt: DateTime.UtcNow,
                        SlotStartTime: slotStart
                    ));
                }
            }

            Status = BookingStatus.Cancelled;
            CancellationReason = reason;
            CancelledBy = cancelledBy;
            Touch();
            RaiseDomainEvent(new BookingCancelledEvent(Id, PatientId, reason, cancelledBy));
            return Result.Success();
        }

        public Result Complete()
        {
            if (Status != BookingStatus.Confirmed)
                return Result.Failure(BookingErrors.CannotCompleteNonConfirmed);

            Status = BookingStatus.Completed;
            Touch();
            return Result.Success();
        }

        public Result<Guid> Reschedule(Guid newScheduleId)
        {
            if (Status != BookingStatus.Confirmed)
                return Result<Guid>.Failure(BookingErrors.CannotRescheduleNonConfirmed);

            if (DoctorSchedule.TimeSlot.ToStartDateTime() - DateTime.UtcNow < TimeSpan.FromHours(2))
                return Result<Guid>.Failure(BookingErrors.RescheduleWindowExpired);

            if (RescheduleCount >= 2)
                return Result<Guid>.Failure(BookingErrors.MaxRescheduleReached);

            var oldScheduleId = DoctorScheduleId;

            RescheduleCount++;
            Status = BookingStatus.Rescheduled;
            Touch();
            RaiseDomainEvent(new BookingRescheduledEvent(Id, PatientId, oldScheduleId, newScheduleId));

            return Result<Guid>.Success(oldScheduleId);
        }

        public Result<Guid> RescheduleByDoctor(Guid newScheduleId)
        {
            if (Status != BookingStatus.Confirmed)
                return Result<Guid>.Failure(BookingErrors.CannotRescheduleNonConfirmed);

            if (DoctorSchedule.TimeSlot.ToStartDateTime() - DateTime.UtcNow < TimeSpan.FromHours(2))
                return Result<Guid>.Failure(BookingErrors.RescheduleWindowExpired);

            if (DoctorRescheduleCount >= 1)
                return Result<Guid>.Failure(BookingErrors.DoctorMaxRescheduleReached);

            var oldScheduleId = DoctorScheduleId;

            DoctorRescheduleCount++;
            Status = BookingStatus.Rescheduled;
            Touch();
            RaiseDomainEvent(new BookingRescheduledEvent(Id, PatientId, oldScheduleId, newScheduleId));

            return Result<Guid>.Success(oldScheduleId);
        }

        public static Booking CreateRescheduled(
            Guid patientId,
            Guid scheduleId,
            string? notes,
            Guid rescheduledFromId,
            int rescheduleCount,
            int doctorRescheduleCount)
        {
            var booking = new Booking
            {
                PatientId = patientId,
                DoctorScheduleId = scheduleId,
                Status = BookingStatus.Confirmed,
                Notes = notes?.Trim(),
                RescheduledFromId = rescheduledFromId,
                RescheduleCount = rescheduleCount,
                DoctorRescheduleCount = doctorRescheduleCount
            };

            booking.Touch();
            booking.RaiseDomainEvent(new BookingCreatedEvent(booking.Id, patientId, scheduleId));
            return booking;
        }

        public Result AutoExpire()
        {
            if (Status != BookingStatus.Pending)
                return Result.Failure(BookingErrors.AlreadyFinalized);

            Status = BookingStatus.Cancelled;
            CancelledBy = Enums.CancelledBy.System;
            CancellationReason = "Booking tự động hủy do không được xác nhận trong 24 giờ.";
            Touch();
            RaiseDomainEvent(new BookingAutoExpiredEvent(Id, PatientId, DoctorScheduleId));
            return Result.Success();
        }
    }
}