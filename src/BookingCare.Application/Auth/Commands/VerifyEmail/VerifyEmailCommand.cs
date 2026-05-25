using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.VerifyEmail
{
    public record VerifyEmailCommand(string Token) : IRequest<Result>;
}
