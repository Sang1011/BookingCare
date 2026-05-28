using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookingCare.Worker.Consumers;

public class DoctorScheduleCreatedConsumer(
    IEmailService emailService,
    ILogger<DoctorScheduleCreatedConsumer> logger)
    : IConsumer<DoctorScheduleCreatedMessage>
{
    public async Task Consume(ConsumeContext<DoctorScheduleCreatedMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Consuming DoctorScheduleCreatedMessage. DoctorId: {DoctorId}", msg.DoctorId);

        const string subject = "📅 Lịch làm việc mới đã được tạo - BookingCare";
        var body = $"""
            <h2>Lịch làm việc mới</h2>
            <p>Xin chào bác sĩ <strong>{msg.DoctorName}</strong>,</p>
            <p>Lịch làm việc của bạn đã được tạo thành công.</p>
            <p>Ngày làm việc: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
            <p>Giờ bắt đầu: <strong>{msg.SlotStart:HH:mm}</strong></p>
            <p>Giờ kết thúc: <strong>{msg.SlotEnd:HH:mm}</strong></p>
            <p>Bệnh nhân có thể đặt lịch hẹn với bạn trong khung giờ này.</p>
            """;

        await emailService.SendAsync(msg.DoctorEmail, subject, body, context.CancellationToken);

        logger.LogInformation(
            "DoctorScheduleCreated email sent. DoctorId: {DoctorId}", msg.DoctorId);
    }
}