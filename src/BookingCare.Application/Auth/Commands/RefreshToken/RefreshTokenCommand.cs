using BookingCare.Application.Common.Models;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        string AccessToken,
        string RefreshToken
    ) : IRequest<Result<TokenResponse>>;
}
