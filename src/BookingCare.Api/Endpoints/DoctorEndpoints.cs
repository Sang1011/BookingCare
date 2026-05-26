using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.Doctors.Commands.CreateSchedule;
using BookingCare.Application.Modules.Doctors.Commands.DeleteSchedule;
using BookingCare.Application.Modules.Doctors.Commands.VerifyDoctor;
using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Application.Modules.Doctors.Queries.GetAvailableSlots;
using BookingCare.Application.Modules.Doctors.Queries.GetDoctorById;
using BookingCare.Application.Modules.Doctors.Queries.GetDoctors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Api.Endpoints
{
    public static class DoctorEndpoints
    {
        public static IEndpointRouteBuilder MapDoctorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/doctors").WithTags("Doctors");

            group.MapGet("/", GetDoctors)
                .WithSummary("Lấy danh sách bác sĩ")
                .WithDescription("Hỗ trợ filter theo tên, chuyên khoa, ngày có lịch trống và phân trang")
                .Produces<PagedResult<DoctorDto>>(200);

            group.MapGet("/{id:guid}", GetDoctorById)
                .WithSummary("Lấy thông tin bác sĩ theo ID")
                .Produces<DoctorDto>(200)
                .Produces<ProblemDetails>(404);

            group.MapGet("/{id:guid}/available-slots", GetAvailableSlots)
                .WithSummary("Lấy các slot khám còn trống của bác sĩ")
                .WithDescription("Lọc theo ngày cụ thể")
                .Produces<List<AvailableSlotDto>>(200)
                .Produces<ProblemDetails>(404);

            group.MapPost("/{id:guid}/schedules", CreateSchedule)
                .WithSummary("Tạo lịch khám cho bác sĩ")
                .WithDescription("Chỉ Doctor (chính mình) hoặc Admin mới có quyền tạo")
                .Produces<DoctorScheduleDto>(201)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(403)
                .RequireAuthorization();

            group.MapDelete("/schedules/{scheduleId:guid}", DeleteSchedule)
                .WithSummary("Xóa lịch khám")
                .WithDescription("Chỉ Doctor (chính mình) hoặc Admin mới có quyền xóa. Không thể xóa nếu đã có booking")
                .Produces(204)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(403)
                .Produces<ProblemDetails>(404)
                .RequireAuthorization();

            group.MapPost("/{id:guid}/verify", VerifyDoctor)
                .WithSummary("Xác minh bác sĩ")
                .WithDescription("Chỉ Admin mới có quyền verify")
                .Produces(204)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(403)
                .Produces<ProblemDetails>(404)
                .RequireAuthorization("Admin");

            return app;
        }
        private static async Task<IResult> GetDoctors(
            ISender sender,
            [FromQuery] string? searchName = null,
            [FromQuery] Guid? specialtyId = null,
            [FromQuery] DateOnly? availableDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetDoctorsQuery(searchName, specialtyId, availableDate, page, pageSize);
            var result = await sender.Send(query);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> GetDoctorById(
            Guid id, ISender sender)
        {
            var result = await sender.Send(new GetDoctorByIdQuery(id));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 404);
        }

        private static async Task<IResult> GetAvailableSlots(
            Guid id, ISender sender,
            [FromQuery] DateOnly? date = null)
        {
            var query = new GetAvailableSlotsQuery(id, date ?? DateOnly.FromDateTime(DateTime.UtcNow));
            var result = await sender.Send(query);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 404);
        }

        private static async Task<IResult> CreateSchedule(
            Guid id, CreateScheduleCommand request, ISender sender)
        {
            var command = new CreateScheduleCommand(
                id,
                request.WorkDate,
                request.SlotStart,
                request.SlotEnd,
                request.MaxPatients);

            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Created($"/api/v1/doctors/{id}/schedules/{result.Value!.Id}", result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> DeleteSchedule(
            Guid scheduleId, ISender sender)
        {
            var result = await sender.Send(new DeleteScheduleCommand(scheduleId));
            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> VerifyDoctor(
            Guid id, ISender sender)
        {
            var result = await sender.Send(new VerifyDoctorCommand(id));
            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }
    }
}