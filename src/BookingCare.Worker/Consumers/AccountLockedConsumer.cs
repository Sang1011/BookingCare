using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookingCare.Worker.Consumers;

public class AccountLockedConsumer(
    IEmailService emailService,
    ILogger<AccountLockedConsumer> logger)
    : IConsumer<AccountLockedMessage>
{
    public async Task Consume(ConsumeContext<AccountLockedMessage> context)
    {
        var msg = context.Message;

        logger.LogInformation("Consuming AccountLockedMessage. Email: {Email}", msg.Email);

        const string subject = "🔒 Tài khoản bị tạm khóa - BookingCare";
        var body = $"""
            <h2>Tài khoản bị tạm khóa</h2>
            <p>Tài khoản <strong>{msg.Email}</strong> bị khóa đến <strong>{msg.LockedUntil:HH:mm dd/MM/yyyy}</strong>
            do đăng nhập sai quá nhiều lần.</p>
            <p>Nếu không phải bạn, hãy liên hệ hỗ trợ ngay.</p>
            """;

        await emailService.SendAsync(msg.Email, subject, body, context.CancellationToken);

        logger.LogInformation("AccountLocked email sent. Email: {Email}", msg.Email);
    }
}