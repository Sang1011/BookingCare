using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Doctor;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using BookingCare.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

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
        public DoctorSchedule DoctorSchedule { get; private set; } = default!;

        private Booking() { }

        public static Booking Create(Guid patientId, Guid scheduleId, string? notes)
        {
            var booking = new Booking
            {
                PatientId = patientId,
                DoctorScheduleId = scheduleId,
                Status = BookingStatus.Pending,
                Notes = notes
            };

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

            // Patient chỉ được hủy nếu còn hơn 2 tiếng
            if (cancelledBy == Enums.CancelledBy.Patient
                && Status == BookingStatus.Confirmed)
            {
                var appointmentTime = DoctorSchedule.WorkDate.ToDateTime(DoctorSchedule.SlotStart);
                if (appointmentTime - DateTime.UtcNow < TimeSpan.FromHours(2))
                    return Result.Failure(BookingErrors.CancellationWindowExpired);
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
    }
}
