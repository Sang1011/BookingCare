using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Messages;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Auth.EventHandlers;

public class AccountLockedEventHandler(
    IMessagePublisher publisher,
    ILogger<AccountLockedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<AccountLockedEvent>>
{
    public async Task Handle(
        DomainEventNotification<AccountLockedEvent> notification,
        CancellationToken ct)
    {
        var e = notification.DomainEvent;

        logger.LogWarning(
            "Account locked for email: {Email}, locked until: {LockedUntil}",
            e.Email, e.LockedUntil);

        await publisher.PublishAsync(new AccountLockedMessage(e.Email, e.LockedUntil), ct);

        logger.LogInformation("AccountLockedMessage published. Email: {Email}", e.Email);
    }
}