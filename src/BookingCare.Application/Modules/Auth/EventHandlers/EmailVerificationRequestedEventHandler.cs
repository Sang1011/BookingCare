using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Messages;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Auth.EventHandlers;

public class EmailVerificationRequestedEventHandler(
    IEmailVerificationService emailVerificationService,
    IMessagePublisher publisher,
    ILogger<EmailVerificationRequestedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<EmailVerificationRequestedEvent>>
{
    public async Task Handle(
        DomainEventNotification<EmailVerificationRequestedEvent> notification,
        CancellationToken ct)
    {
        var e = notification.DomainEvent;

        logger.LogInformation(
            "Generating verification token for userId: {UserId}, email: {Email}",
            e.UserId, e.Email);

        var token = await emailVerificationService.GenerateTokenAsync(e.UserId, ct);

        await publisher.PublishAsync(new EmailVerificationRequestedMessage(
            e.UserId,
            e.Email,
            token), ct);

        logger.LogInformation(
            "EmailVerificationRequestedMessage published. Email: {Email}", e.Email);
    }
}