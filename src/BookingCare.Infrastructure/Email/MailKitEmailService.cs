using BookingCare.Application.Common.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BookingCare.Infrastructure.Email;

public class MailKitEmailService(
    IOptions<EmailSettings> options,
    ILogger<MailKitEmailService> logger)
    : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        logger.LogInformation("Sending email to {To}, subject: {Subject}", to, subject);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            TextBody = HtmlToPlainText(body),
            HtmlBody = WrapHtml(subject, body)
        };

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("Email sent successfully to {To}", to);
    }

    public async Task SendToManyAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken ct = default)
    {
        var emailList = recipients.ToList();
        if (emailList.Count == 0) return;

        logger.LogInformation("Sending bulk email to {Count} recipients, subject: {Subject}", emailList.Count, subject);

        await Task.WhenAll(emailList.Select(to => SendAsync(to, subject, body, ct)));

        logger.LogInformation("Bulk email sent to {Count} recipients", emailList.Count);
    }

    private static string WrapHtml(string subject, string content)
    {
        var year = DateTime.UtcNow.Year;
        return """
            <!DOCTYPE html>
            <html lang="vi">
            <head>
              <meta charset="UTF-8" />
              <meta name="viewport" content="width=device-width, initial-scale=1.0" />
              <title>
            """ + subject + """
              </title>
              <style>
                body {
                  margin: 0; padding: 0;
                  background-color: #f4f4f7;
                  font-family: Arial, sans-serif;
                  color: #333;
                }
                .wrapper {
                  max-width: 600px;
                  margin: 40px auto;
                  background: #ffffff;
                  border-radius: 8px;
                  overflow: hidden;
                  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
                }
                .header {
                  background-color: #2563eb;
                  padding: 24px 32px;
                  color: #fff;
                  font-size: 20px;
                  font-weight: bold;
                }
                .body {
                  padding: 32px;
                  font-size: 15px;
                  line-height: 1.7;
                }
                .body h2 {
                  margin-top: 0;
                  color: #1e293b;
                }
                .body a {
                  display: inline-block;
                  margin-top: 12px;
                  padding: 12px 24px;
                  background-color: #2563eb;
                  color: #fff;
                  border-radius: 6px;
                  text-decoration: none;
                  font-weight: bold;
                }
                .footer {
                  background-color: #f4f4f7;
                  padding: 16px 32px;
                  font-size: 12px;
                  color: #888;
                  text-align: center;
                }
              </style>
            </head>
            <body>
              <div class="wrapper">
                <div class="header">BookingCare</div>
                <div class="body">
            """ + content + """
                </div>
                <div class="footer">
                  &copy; 
            """ + year + """
                   BookingCare. Email này được gửi tự động, vui lòng không trả lời.
                </div>
              </div>
            </body>
            </html>
            """;
    }

    private static string HtmlToPlainText(string html)
    {
        var text = System.Text.RegularExpressions.Regex.Replace(
            html,
            @"<(br|p|h[1-6]|li|div|tr)[^>]*>",
            "\n",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        text = System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]+>", string.Empty);
        text = System.Net.WebUtility.HtmlDecode(text);
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\n{3,}", "\n\n");

        return text.Trim();
    }
}