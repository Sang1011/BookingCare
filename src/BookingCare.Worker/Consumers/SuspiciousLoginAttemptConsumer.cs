using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookingCare.Worker.Consumers;

public class SuspiciousLoginAttemptConsumer(
    IEmailService emailService,
    ILogger<SuspiciousLoginAttemptConsumer> logger)
    : IConsumer<SuspiciousLoginAttemptMessage>
{
    public async Task Consume(ConsumeContext<SuspiciousLoginAttemptMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Consuming SuspiciousLoginAttemptMessage. Email: {Email}", msg.Email);

        const string subject = "⚠️ Cảnh báo bảo mật - BookingCare";
        var body = $"""
            <h2>Cảnh báo bảo mật</h2>
            <p>Có ai đó đang cố đăng nhập vào tài khoản <strong>{msg.Email}</strong>.</p>
            <p>Nếu không phải bạn, hãy đổi mật khẩu ngay lập tức.</p>
            """;

        await emailService.SendAsync(msg.Email, subject, body, context.CancellationToken);

        logger.LogInformation(
            "SuspiciousLoginAttempt email sent. Email: {Email}", msg.Email);
    }
}