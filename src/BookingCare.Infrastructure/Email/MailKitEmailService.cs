using BookingCare.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BookingCare.Infrastructure.Email
{
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
    }
}
