using BookingCare.Application.Common.Interfaces.Services;
using MassTransit;

namespace BookingCare.Infrastructure.Messaging;

public class MassTransitMessagePublisher(IPublishEndpoint publishEndpoint) : IMessagePublisher
{
    public async Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class
        => await publishEndpoint.Publish(message, ct);
}