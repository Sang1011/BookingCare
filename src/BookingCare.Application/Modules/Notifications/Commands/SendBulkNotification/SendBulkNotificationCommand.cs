using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Notifications.Commands.SendBulkNotification;

public record SendBulkNotificationCommand(
    string Subject,
    string Body,
    bool SendToAll = false,
    IEnumerable<string>? Recipients = null
) : IRequest<Result>;