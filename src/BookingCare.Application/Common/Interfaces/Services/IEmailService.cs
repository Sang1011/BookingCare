using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
        Task SendBookingConfirmedAsync(string patientEmail, string doctorName,
            DateOnly workDate, TimeOnly slotStart, CancellationToken ct = default);
        Task SendBookingCancelledAsync(string patientEmail, string doctorName,
            DateOnly workDate, TimeOnly slotStart, string reason, CancellationToken ct = default);
        Task SendBookingReminderAsync(string patientEmail, string doctorName,
            DateOnly workDate, TimeOnly slotStart, CancellationToken ct = default);
    }
}
