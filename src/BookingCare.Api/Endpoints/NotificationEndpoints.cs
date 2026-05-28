using BookingCare.Application.Modules.Notifications.Commands.SendBulkNotification;
using BookingCare.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Api.Endpoints;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/notifications")
            .WithTags("Notifications")
            .RequireAuthorization(UserRole.Admin.ToString());

        group.MapPost("/send", SendBulkNotification)
            .WithSummary("Gửi thông báo hàng loạt")
            .WithDescription("Gửi email đến danh sách cụ thể hoặc tất cả user (trừ Admin). SendToAll = true thì không cần truyền Recipients")
            .Produces(200)
            .Produces<ProblemDetails>(400);

        return app;
    }

    private static async Task<IResult> SendBulkNotification(
        [FromBody] SendBulkNotificationCommand command, ISender sender)
    {
        var result = await sender.Send(command);
        return result.IsSuccess
            ? Results.Ok()
            : Results.Problem(result.Error!.Message, statusCode: 400);
    }
}