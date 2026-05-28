using BookingCare.Application.Common.Models;
using BookingCare.Application.Modules.MedicalRecords.Commands.CreateMedicalRecord;
using BookingCare.Application.Modules.MedicalRecords.Commands.UpdateMedicalRecord;
using BookingCare.Application.Modules.MedicalRecords.DTOs;
using BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecordByBookingId;
using BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecordById;
using BookingCare.Application.Modules.MedicalRecords.Queries.GetMedicalRecords;
using BookingCare.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Api.Endpoints
{
    public static class MedicalRecordEndpoints
    {
        public static IEndpointRouteBuilder MapMedicalRecordEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/medical-records")
                .WithTags("MedicalRecords")
                .RequireAuthorization();

            group.MapPost("/", CreateMedicalRecord)
                .WithSummary("Tạo hồ sơ bệnh án")
                .WithDescription("Chỉ Doctor của booking đó mới được tạo, booking phải ở trạng thái Completed")
                .Produces<MedicalRecordDto>(201)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(409)
                .RequireAuthorization(UserRole.Doctor.ToString());

            group.MapPut("/{id:guid}", UpdateMedicalRecord)
                .WithSummary("Cập nhật hồ sơ bệnh án")
                .WithDescription("Chỉ Doctor tạo record hoặc Admin")
                .Produces(204)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(404)
                .RequireAuthorization(MultiRole.DoctorOrAdmin.ToString());

            group.MapGet("/{id:guid}", GetMedicalRecordById)
                .WithSummary("Chi tiết hồ sơ bệnh án")
                .WithDescription("Patient chỉ xem được của mình")
                .Produces<MedicalRecordDto>(200)
                .Produces<ProblemDetails>(404)
                .RequireAuthorization(MultiRole.PatientOrDoctor.ToString());

            group.MapGet("/", GetMedicalRecords)
                .WithSummary("Danh sách hồ sơ bệnh án")
                .WithDescription("Patient xem của mình, Admin xem tất cả")
                .Produces<PagedResult<MedicalRecordDto>>(200)
                .RequireAuthorization(MultiRole.PatientOrAdmin.ToString());

            group.MapGet("/by-booking/{bookingId:guid}", GetByBookingId)
                .WithSummary("Hồ sơ bệnh án theo booking")
                .WithDescription("Tra cứu nhanh từ bookingId")
                .Produces<MedicalRecordDto>(200)
                .Produces<ProblemDetails>(404)
                .RequireAuthorization(MultiRole.PatientOrDoctor.ToString());

            return app;
        }

        private static async Task<IResult> CreateMedicalRecord(
            CreateMedicalRecordCommand command, ISender sender)
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Created($"/api/v1/medical-records/{result.Value!.Id}", result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> UpdateMedicalRecord(
            Guid id, [FromBody] UpdateMedicalRecordCommand command, ISender sender)
        {
            var result = await sender.Send(command with { Id = id });
            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> GetMedicalRecordById(
            Guid id, ISender sender)
        {
            var result = await sender.Send(new GetMedicalRecordByIdQuery(id));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 404);
        }

        private static async Task<IResult> GetMedicalRecords(
            ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await sender.Send(new GetMedicalRecordsQuery(page, pageSize));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> GetByBookingId(
            Guid bookingId, ISender sender)
        {
            var result = await sender.Send(new GetMedicalRecordByBookingIdQuery(bookingId));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 404);
        }
    }
}