using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Auth.Commands.Logout
{
    public record LogoutCommand : IRequest<Result>;
}
