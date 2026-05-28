using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Messages;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Auth.EventHandlers;

public class SuspiciousLoginAttemptEventHandler(
    IMessagePublisher publisher,
    ILogger<SuspiciousLoginAttemptEventHandler> logger)
    : INotificationHandler<DomainEventNotification<SuspiciousLoginAttemptEvent>>
{
    public async Task Handle(
        DomainEventNotification<SuspiciousLoginAttemptEvent> notification,
        CancellationToken ct)
    {
        var e = notification.DomainEvent;

        logger.LogWarning(
            "Suspicious login attempt detected for email: {Email}", e.Email);

        await publisher.PublishAsync(new SuspiciousLoginAttemptMessage(e.Email), ct);

        logger.LogInformation("SuspiciousLoginAttemptMessage published. Email: {Email}", e.Email);
    }
}