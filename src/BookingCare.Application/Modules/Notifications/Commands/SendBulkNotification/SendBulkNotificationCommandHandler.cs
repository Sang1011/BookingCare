using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Notifications.Commands.SendBulkNotification;

public class SendBulkNotificationCommandHandler(
    IEmailService emailService,
    IUserRepository userRepository,
    ILogger<SendBulkNotificationCommandHandler> logger)
    : IRequestHandler<SendBulkNotificationCommand, Result>
{
    public async Task<Result> Handle(SendBulkNotificationCommand request, CancellationToken ct)
    {
        List<string> recipients;

        if (request.SendToAll)
        {
            recipients = (await userRepository.GetAllUserEmailsAsync(ct)).ToList();
        }
        else
        {
            if (request.Recipients is null || !request.Recipients.Any())
            {
                logger.LogWarning("SendBulkNotification: no recipients provided");
                return Result.Failure(NotificationErrors.NoRecipientsProvided);
            }

            recipients = request.Recipients.ToList();
        }

        if (recipients.Count == 0)
        {
            logger.LogWarning("SendBulkNotification: recipient list is empty");
            return Result.Success();
        }

        logger.LogInformation(
            "Sending bulk notification to {Count} recipients, subject: {Subject}",
            recipients.Count, request.Subject);

        await emailService.SendToManyAsync(recipients, request.Subject, request.Body, ct);

        logger.LogInformation(
            "Bulk notification sent to {Count} recipients", recipients.Count);

        return Result.Success();
    }
}