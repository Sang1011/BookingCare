using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookingCare.Worker.Consumers;

public class BookingAutoExpiredConsumer(
    IEmailService emailService,
    ILogger<BookingAutoExpiredConsumer> logger)
    : IConsumer<BookingAutoExpiredMessage>
{
    public async Task Consume(ConsumeContext<BookingAutoExpiredMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Consuming BookingAutoExpiredMessage. BookingId: {BookingId}", msg.BookingId);

        const string subject = "⏰ Lịch hẹn đã hết hạn - BookingCare";
        var body = $"""
            <h2>Lịch hẹn đã bị tự động hủy</h2>
            <p>Xin chào <strong>{msg.PatientName}</strong>,</p>
            <p>Bác sĩ: <strong>{msg.DoctorName}</strong></p>
            <p>Ngày khám: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{msg.SlotStart:HH:mm}</strong></p>
            <p>Lịch hẹn bị hủy do chưa được xác nhận sau 24 giờ.</p>
            <p>Vui lòng đặt lịch mới nếu bạn vẫn có nhu cầu khám.</p>
            """;

        await emailService.SendAsync(msg.PatientEmail, subject, body, context.CancellationToken);

        logger.LogInformation(
            "BookingAutoExpired email sent. BookingId: {BookingId}", msg.BookingId);
    }
}