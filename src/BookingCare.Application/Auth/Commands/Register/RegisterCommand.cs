using BookingCare.Application.Auth.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.Register
{
    public record RegisterCommand(
        string Email,
        string Password,
        string FullName,
        string? Phone,
        DateOnly? DateOfBirth
    ) : IRequest<Result<UserDto>>;
}
