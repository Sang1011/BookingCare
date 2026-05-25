using BookingCare.Application.Auth.Commands.ChangePassword;
using BookingCare.Application.Auth.Commands.Login;
using BookingCare.Application.Auth.Commands.Logout;
using BookingCare.Application.Auth.Commands.RefreshToken;
using BookingCare.Application.Auth.Commands.Register;
using BookingCare.Application.Auth.Commands.RegisterDoctor;
using BookingCare.Application.Auth.Commands.VerifyEmail;
using BookingCare.Application.Auth.DTOs;
using BookingCare.Application.Auth.Queries.GetCurrentUser;
using BookingCare.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

            group.MapPost("/register", Register)
                .WithSummary("Đăng ký tài khoản Patient")
                .WithDescription("Tạo tài khoản mới với role Patient, gửi email xác nhận sau khi đăng ký")
                .Produces<UserDto>(201)
                .Produces<ProblemDetails>(400);

            group.MapPost("/login", Login)
                .WithSummary("Đăng nhập")
                .WithDescription("Trả về Access Token và Refresh Token")
                .Produces<TokenResponse>(200)
                .Produces<ProblemDetails>(401);

            group.MapPost("/refresh-token", RefreshToken)
                .WithSummary("Làm mới Access Token")
                .WithDescription("Dùng Refresh Token để lấy Access Token mới")
                .Produces<TokenResponse>(200)
                .Produces<ProblemDetails>(401);

            group.MapGet("/me", GetCurrentUser)
                .WithSummary("Lấy thông tin user hiện tại")
                .Produces<UserDto>(200)
                .Produces<ProblemDetails>(401)
                .Produces<ProblemDetails>(404)
                .RequireAuthorization();

            group.MapPost("/logout", Logout)
                .WithSummary("Đăng xuất")
                .WithDescription("Revoke tất cả Refresh Token của user")
                .Produces(204)
                .Produces<ProblemDetails>(401)
                .RequireAuthorization();

            group.MapPost("/change-password", ChangePassword)
                .WithSummary("Đổi mật khẩu")
                .Produces(204)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(401)
                .RequireAuthorization();

            group.MapPost("/register/doctor", RegisterDoctor)
                .WithSummary("Đăng ký tài khoản Doctor")
                .WithDescription("Chỉ Admin mới có quyền tạo tài khoản Doctor")
                .Produces<UserDto>(201)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(403)
                .RequireAuthorization();

            group.MapPost("/verify-email", VerifyEmail)
                .WithSummary("Xác nhận email")
                .WithDescription("Xác nhận email bằng token nhận được qua email sau khi đăng ký")
                .Produces(200)
                .Produces<ProblemDetails>(400);
            return app;
        }

        private static async Task<IResult> Register(
            RegisterCommand command, ISender sender)
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Created($"/api/v1/users/{result.Value!.Id}", result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> Login(
            LoginCommand command, ISender sender)
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 401);
        }

        private static async Task<IResult> RefreshToken(
            RefreshTokenCommand command, ISender sender)
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 401);
        }

        private static async Task<IResult> Logout(ISender sender)
        {
            var result = await sender.Send(new LogoutCommand());
            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> ChangePassword(
            ChangePasswordCommand command, ISender sender)
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> RegisterDoctor(
            RegisterDoctorCommand command, ISender sender)
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Created($"/api/v1/users/{result.Value!.Id}", result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

        private static async Task<IResult> GetCurrentUser(ISender sender)
        {
            var result = await sender.Send(new GetCurrentUserQuery());
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error!.Message, statusCode: 404);
        }

        private static async Task<IResult> VerifyEmail(
            VerifyEmailCommand command, ISender sender)
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok("Email xác nhận thành công!")
                : Results.Problem(result.Error!.Message, statusCode: 400);
        }

    }
}