using BookingCare.Application.Common.Models;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.Login
{
    public record LoginCommand(
        string Email,
        string Password
    ) : IRequest<Result<TokenResponse>>;
}
