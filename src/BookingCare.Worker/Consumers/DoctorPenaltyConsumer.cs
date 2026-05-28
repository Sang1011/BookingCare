using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookingCare.Worker.Consumers;

public class DoctorPenaltyConsumer(
    IEmailService emailService,
    IUserRepository userRepository,
    ILogger<DoctorPenaltyConsumer> logger)
    : IConsumer<DoctorPenaltyMessage>
{
    public async Task Consume(ConsumeContext<DoctorPenaltyMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Consuming DoctorPenaltyMessage. BookingId: {BookingId}", msg.BookingId);

        var tasks = new List<Task>();

        // Gửi mail Doctor
        const string doctorSubject = "⚠️ Thông báo vi phạm chính sách hủy lịch - BookingCare";
        var doctorBody = $"""
            <h2>Thông báo vi phạm</h2>
            <p>Xin chào bác sĩ <strong>{msg.DoctorName}</strong>,</p>
            <p>Hệ thống ghi nhận bạn đã hủy lịch đã xác nhận trong vòng 2 giờ trước giờ khám.</p>
            <p>Ngày khám: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{msg.SlotStart:HH:mm}</strong></p>
            <p>Thời điểm hủy: <strong>{msg.CancelledAt:HH:mm dd/MM/yyyy}</strong></p>
            <p>Hành động này ảnh hưởng đến bệnh nhân và có thể dẫn đến các biện pháp xử lý từ phía hệ thống.</p>
            """;
        tasks.Add(emailService.SendAsync(msg.DoctorEmail, doctorSubject, doctorBody, context.CancellationToken));

        // Gửi mail Patient
        const string patientSubject = "❌ Lịch hẹn của bạn đã bị hủy bởi bác sĩ - BookingCare";
        var patientBody = $"""
            <h2>Thông báo hủy lịch hẹn</h2>
            <p>Xin chào <strong>{msg.PatientName}</strong>,</p>
            <p>Rất tiếc, bác sĩ <strong>{msg.DoctorName}</strong> đã hủy lịch hẹn của bạn.</p>
            <p>Ngày khám: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{msg.SlotStart:HH:mm}</strong></p>
            <p>Bạn có thể đặt lịch mới với bác sĩ khác hoặc liên hệ hỗ trợ.</p>
            """;
        tasks.Add(emailService.SendAsync(msg.PatientEmail, patientSubject, patientBody, context.CancellationToken));

        // Gửi mail tất cả Admin
        var adminEmails = await userRepository.GetAdminEmailsAsync(context.CancellationToken);
        const string adminSubject = "⚠️ Cảnh báo: Bác sĩ hủy lịch sát giờ khám - BookingCare";
        var adminBody = $"""
            <h2>Cảnh báo vi phạm</h2>
            <p>Bác sĩ <strong>{msg.DoctorName}</strong> đã hủy lịch đã xác nhận trong vòng 2 giờ trước giờ khám.</p>
            <p>Mã booking: <strong>{msg.BookingId}</strong></p>
            <p>Bệnh nhân: <strong>{msg.PatientName}</strong></p>
            <p>Ngày khám: <strong>{msg.WorkDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{msg.SlotStart:HH:mm}</strong></p>
            <p>Thời điểm hủy: <strong>{msg.CancelledAt:HH:mm dd/MM/yyyy}</strong></p>
            <p>Vui lòng xem xét và xử lý nếu cần thiết.</p>
            """;
        tasks.AddRange(adminEmails.Select(email =>
            emailService.SendAsync(email, adminSubject, adminBody, context.CancellationToken)));

        await Task.WhenAll(tasks);

        logger.LogInformation(
            "DoctorPenalty emails sent. BookingId: {BookingId}, AdminCount: {Count}",
            msg.BookingId, adminEmails.Count);
    }
}