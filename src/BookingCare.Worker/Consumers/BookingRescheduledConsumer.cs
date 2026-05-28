using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookingCare.Worker.Consumers;

public class BookingRescheduledConsumer(
    IEmailService emailService,
    ILogger<BookingRescheduledConsumer> logger)
    : IConsumer<BookingRescheduledMessage>
{
    public async Task Consume(ConsumeContext<BookingRescheduledMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Consuming BookingRescheduledMessage. BookingId: {BookingId}", msg.BookingId);

        const string subject = "🔄 Lịch hẹn đã được dời - BookingCare";
        var body = $"""
            <h2>Lịch hẹn đã được dời sang slot mới</h2>
            <p>Xin chào <strong>{msg.PatientName}</strong>,</p>
            <p>Lịch hẹn với bác sĩ <strong>{msg.DoctorName}</strong> đã được dời thành công.</p>
            <p>Ngày khám mới: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám mới: <strong>{msg.SlotStart:HH:mm} - {msg.SlotEnd:HH:mm}</strong></p>
            <p>Vui lòng có mặt trước 15 phút.</p>
            """;

        await emailService.SendAsync(msg.PatientEmail, subject, body, context.CancellationToken);

        logger.LogInformation(
            "BookingRescheduled email sent. BookingId: {BookingId}", msg.BookingId);
    }
}