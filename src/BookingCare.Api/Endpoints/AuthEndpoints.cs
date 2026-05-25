using BookingCare.Application.Auth.Commands.ChangePassword;
using BookingCare.Application.Auth.Commands.Login;
using BookingCare.Application.Auth.Commands.Logout;
using BookingCare.Application.Auth.Commands.Register;
using BookingCare.Application.Auth.Commands.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

            group.MapPost("/register", Register);
            group.MapPost("/login", Login);
            group.MapPost("/refresh-token", RefreshToken);
            group.MapPost("/logout", Logout).RequireAuthorization();
            group.MapPost("/change-password", ChangePassword).RequireAuthorization();

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
    }
}