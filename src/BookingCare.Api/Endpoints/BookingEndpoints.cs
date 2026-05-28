
using BookingCare.Application.Bookings.Queries.GetBookingById;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.Bookings.Commands.CancelBooking;
using BookingCare.Application.Modules.Bookings.Commands.CompleteBooking;
using BookingCare.Application.Modules.Bookings.Commands.ConfirmBooking;
using BookingCare.Application.Modules.Bookings.Commands.CreateBooking;
using BookingCare.Application.Modules.Bookings.Commands.RescheduleBooking;
using BookingCare.Application.Modules.Bookings.DTOs;
using BookingCare.Application.Modules.Bookings.Queries.GetBookings;
using BookingCare.Application.Modules.Bookings.Queries.GetBookingsByDoctor;
using BookingCare.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Api.Endpoints;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/bookings")
            .WithTags("Bookings")
            .RequireAuthorization();

        group.MapPost("/", CreateBooking)
            .WithSummary("Tạo lịch hẹn mới")
            .WithDescription("Chỉ Patient mới có thể đặt lịch")
            .RequireAuthorization(UserRole.Patient.ToString())
            .Produces<Guid>(201)
            .Produces<ProblemDetails>(400)
            .Produces<ProblemDetails>(409);

        group.MapGet("/", GetBookings)
            .WithSummary("Danh sách lịch hẹn")
            .WithDescription("Patient xem của mình, Doctor xem booking thuộc lịch của mình")
            .Produces<PagedResult<BookingDto>>(200)
            .RequireAuthorization(MultiRole.PatientOrDoctor.ToString());

        group.MapGet("/{id:guid}", GetBookingById)
            .WithSummary("Chi tiết lịch hẹn")
            .Produces<BookingDetailDto>(200)
            .Produces<ProblemDetails>(404);

        group.MapPut("/{id:guid}/confirm", ConfirmBooking)
            .WithSummary("Xác nhận lịch hẹn")
            .WithDescription("Chỉ Doctor (của lịch đó) hoặc Admin mới có quyền xác nhận")
            .RequireAuthorization(MultiRole.DoctorOrAdmin.ToString())
            .Produces(204)
            .Produces<ProblemDetails>(400)
            .Produces<ProblemDetails>(404);

        group.MapPut("/{id:guid}/cancel", CancelBooking)
            .WithSummary("Hủy lịch hẹn")
            .WithDescription("Patient hủy booking của mình, Doctor/Admin hủy bắt buộc phải có lý do")
            .Produces(204)
            .Produces<ProblemDetails>(400)
            .Produces<ProblemDetails>(404);

        group.MapPut("/{id:guid}/reschedule", RescheduleBooking)
            .WithSummary("Dời lịch hẹn sang slot khác")
            .WithDescription("Chỉ Patient mới được dời, slot mới phải cùng bác sĩ")
            .Produces<Guid>(200)
            .Produces<ProblemDetails>(400)
            .Produces<ProblemDetails>(404)
            .RequireAuthorization(UserRole.Patient.ToString());

        group.MapPut("/{id:guid}/complete", CompleteBooking)
            .WithSummary("Đánh dấu hoàn thành")
            .WithDescription("Chỉ Doctor (của lịch đó) hoặc Admin mới có quyền")
            .Produces(204)
            .Produces<ProblemDetails>(400)
            .Produces<ProblemDetails>(404)
            .RequireAuthorization(MultiRole.DoctorOrAdmin.ToString());

        group.MapGet("/doctor/{doctorId:guid}", GetBookingsByDoctor)
            .WithSummary("Danh sách booking của một bác sĩ")
            .WithDescription("Doctor xem lịch của mình, Admin xem của bất kỳ bác sĩ nào")
            .Produces<PagedResult<BookingDto>>(200)
            .Produces<ProblemDetails>(403)
            .RequireAuthorization(MultiRole.DoctorOrAdmin.ToString());

        return app;
    }

    private static async Task<IResult> CreateBooking(
        CreateBookingCommand command, ISender sender)
    {
        var result = await sender.Send(command);
        return result.IsSuccess
            ? Results.Created($"/api/v1/bookings/{result.Value}", result.Value)
            : Results.Problem(result.Error!.Message, statusCode: 409);
    }

    private static async Task<IResult> GetBookings(
        ISender sender,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] BookingStatus? status = null)
    {
        var result = await sender.Send(new GetBookingsQuery(page, pageSize, status));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error!.Message, statusCode: 400);
    }

    private static async Task<IResult> GetBookingById(
        Guid id, ISender sender)
    {
        var result = await sender.Send(new GetBookingByIdQuery(id));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error!.Message, statusCode: 404);
    }

    private static async Task<IResult> ConfirmBooking(
        Guid id, ISender sender)
    {
        var result = await sender.Send(new ConfirmBookingCommand(id));
        return result.IsSuccess
            ? Results.NoContent()
            : Results.Problem(result.Error!.Message, statusCode: 400);
    }

    private static async Task<IResult> CancelBooking(
    Guid id, [FromBody] CancelBookingCommand command, ISender sender)
    {
        var result = await sender.Send(command with { BookingId = id });
        return result.IsSuccess
            ? Results.NoContent()
            : Results.Problem(result.Error!.Message, statusCode: 400);
    }

    private static async Task<IResult> RescheduleBooking(
        Guid id, [FromBody] RescheduleBookingCommand command, ISender sender)
    {
        var result = await sender.Send(command with { BookingId = id });
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error!.Message, statusCode: 400);
    }

    private static async Task<IResult> CompleteBooking(
        Guid id, ISender sender)
    {
        var result = await sender.Send(new CompleteBookingCommand(id));
        return result.IsSuccess
            ? Results.NoContent()
            : Results.Problem(result.Error!.Message, statusCode: 400);
    }

    private static async Task<IResult> GetBookingsByDoctor(
        Guid doctorId, ISender sender,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] DateOnly? date = null)
    {
        var result = await sender.Send(new GetBookingsByDoctorQuery(doctorId, page, pageSize, status, date));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error!.Message, statusCode: 400);
    }
}