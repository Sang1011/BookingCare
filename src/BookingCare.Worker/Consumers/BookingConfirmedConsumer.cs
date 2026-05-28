using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using MassTransit;

namespace BookingCare.Worker.Consumers;

public class BookingConfirmedConsumer(
    IEmailService emailService,
    ILogger<BookingConfirmedConsumer> logger)
    : IConsumer<BookingConfirmedMessage>
{
    public async Task Consume(ConsumeContext<BookingConfirmedMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Consuming BookingConfirmedMessage. BookingId: {BookingId}", msg.BookingId);

        const string subject = "✅ Lịch hẹn của bạn đã được xác nhận - BookingCare";
        var body = $"""
            <h2>Lịch hẹn đã được xác nhận</h2>
            <p>Xin chào <strong>{msg.PatientName}</strong>,</p>
            <p>Bác sĩ: <strong>{msg.DoctorName}</strong></p>
            <p>Ngày khám: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{msg.SlotStart:HH:mm} - {msg.SlotEnd:HH:mm}</strong></p>
            <p>Vui lòng có mặt trước 15 phút.</p>
            """;

        await emailService.SendAsync(msg.PatientEmail, subject, body, context.CancellationToken);

        logger.LogInformation(
            "BookingConfirmed email sent. BookingId: {BookingId}", msg.BookingId);
    }
}