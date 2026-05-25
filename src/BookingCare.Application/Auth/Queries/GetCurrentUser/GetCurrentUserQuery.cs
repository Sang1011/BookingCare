using BookingCare.Application.Auth.DTOs;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Queries.GetCurrentUser
{
    public record GetCurrentUserQuery : IRequest<Result<UserDto>>;
}
