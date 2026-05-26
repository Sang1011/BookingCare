using BookingCare.Application.Common.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BookingCare.Infrastructure.Email;

public class MailKitEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public MailKitEmailService(IOptions<EmailSettings> settings)
        => _settings = settings.Value;

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }

    public Task SendBookingConfirmedAsync(
        string patientEmail,
        string doctorName,
        DateOnly workDate,
        TimeOnly slotStart,
        CancellationToken ct = default)
    {
        var subject = "Xác nhận lịch hẹn khám bệnh";
        var body = $"""
            <h2>Lịch hẹn của bạn đã được xác nhận</h2>
            <p>Bác sĩ: <strong>{doctorName}</strong></p>
            <p>Ngày khám: <strong>{workDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{slotStart:HH:mm}</strong></p>
            <p>Vui lòng có mặt trước 15 phút.</p>
            """;
        return SendAsync(patientEmail, subject, body, ct);
    }

    public Task SendBookingCancelledAsync(
        string patientEmail,
        string doctorName,
        DateOnly workDate,
        TimeOnly slotStart,
        string reason,
        CancellationToken ct = default)
    {
        var subject = "Thông báo hủy lịch hẹn khám bệnh";
        var body = $"""
            <h2>Lịch hẹn của bạn đã bị hủy</h2>
            <p>Bác sĩ: <strong>{doctorName}</strong></p>
            <p>Ngày khám: <strong>{workDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{slotStart:HH:mm}</strong></p>
            <p>Lý do: {reason}</p>
            """;
        return SendAsync(patientEmail, subject, body, ct);
    }

    public Task SendBookingReminderAsync(
        string patientEmail,
        string doctorName,
        DateOnly workDate,
        TimeOnly slotStart,
        CancellationToken ct = default)
    {
        var subject = "Nhắc nhở lịch hẹn khám bệnh ngày mai";
        var body = $"""
            <h2>Nhắc nhở: Bạn có lịch hẹn khám bệnh vào ngày mai</h2>
            <p>Bác sĩ: <strong>{doctorName}</strong></p>
            <p>Ngày khám: <strong>{workDate:dd/MM/yyyy}</strong></p>
            <p>Giờ khám: <strong>{slotStart:HH:mm}</strong></p>
            <p>Vui lòng có mặt trước 15 phút.</p>
            """;
        return SendAsync(patientEmail, subject, body, ct);
    }
}