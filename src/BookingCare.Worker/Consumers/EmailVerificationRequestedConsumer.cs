using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace BookingCare.Worker.Consumers;

public class EmailVerificationRequestedConsumer(
    IEmailService emailService,
    IConfiguration configuration,
    ILogger<EmailVerificationRequestedConsumer> logger)
    : IConsumer<EmailVerificationRequestedMessage>
{
    public async Task Consume(ConsumeContext<EmailVerificationRequestedMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Consuming EmailVerificationRequestedMessage. Email: {Email}", msg.Email);

        var apiBaseUrl = configuration["ApiBaseUrl"];
        const string subject = "Xác nhận tài khoản BookingCare";
        var body = $"""
            <h2>Xin chào!</h2>
            <p>Cảm ơn bạn đã đăng ký tài khoản BookingCare.</p>
            <p>Vui lòng xác nhận email bằng cách bấm vào link bên dưới:</p>
            <a href="{apiBaseUrl}/api/v1/auth/verify-email?token={msg.Token}">
                Xác nhận email
            </a>
            <p>Link có hiệu lực trong 24 giờ.</p>
            """;

        await emailService.SendAsync(msg.Email, subject, body, context.CancellationToken);

        logger.LogInformation(
            "Verification email sent. Email: {Email}", msg.Email);
    }
}