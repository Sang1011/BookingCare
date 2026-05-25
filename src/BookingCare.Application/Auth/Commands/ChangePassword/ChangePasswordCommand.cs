using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword
    ) : IRequest<Result>;
}
