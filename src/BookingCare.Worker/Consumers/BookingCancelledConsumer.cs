using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using BookingCare.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookingCare.Worker.Consumers;

public class BookingCancelledConsumer(
    IEmailService emailService,
    ILogger<BookingCancelledConsumer> logger)
    : IConsumer<BookingCancelledMessage>
{
    public async Task Consume(ConsumeContext<BookingCancelledMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Consuming BookingCancelledMessage. BookingId: {BookingId}, CancelledBy: {CancelledBy}",
            msg.BookingId, msg.CancelledBy);

        var tasks = new List<Task>();

        // Luôn gửi mail Patient
        const string patientSubject = "❌ Thông báo hủy lịch hẹn - BookingCare";
        var patientBody = $"""
            <h2>Lịch hẹn đã bị hủy</h2>
            <p>Xin chào <strong>{msg.PatientName}</strong>,</p>
            <p>Bác sĩ: <strong>{msg.DoctorName}</strong></p>
            <p>Ngày khám: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{msg.SlotStart:HH:mm}</strong></p>
            <p>Lý do: {msg.Reason}</p>
            """;
        tasks.Add(emailService.SendAsync(msg.PatientEmail, patientSubject, patientBody, context.CancellationToken));

        // Nếu Patient hủy thì gửi thêm mail Doctor
        if (msg.CancelledBy == CancelledBy.Patient)
        {
            const string doctorSubject = "📋 Bệnh nhân đã hủy lịch hẹn - BookingCare";
            var doctorBody = $"""
                <h2>Lịch hẹn bị hủy bởi bệnh nhân</h2>
                <p>Bệnh nhân <strong>{msg.PatientName}</strong> đã hủy lịch hẹn.</p>
                <p>Ngày khám: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
                <p>Giờ khám: <strong>{msg.SlotStart:HH:mm}</strong></p>
                <p>Lý do: {msg.Reason}</p>
                """;
            tasks.Add(emailService.SendAsync(msg.DoctorEmail, doctorSubject, doctorBody, context.CancellationToken));
        }

        await Task.WhenAll(tasks);

        logger.LogInformation(
            "BookingCancelled emails sent. BookingId: {BookingId}", msg.BookingId);
    }
}