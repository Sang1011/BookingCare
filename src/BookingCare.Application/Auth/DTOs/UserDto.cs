using BookingCare.Domain.Entities.Auth;
using BookingCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.DTOs
{
    public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string? Phone,
    DateOnly? DateOfBirth,
    string Role 
    )
        {
            public static UserDto FromEntity(User user) => new(
                user.Id,
                user.Email,
                user.FullName,
                user.Phone,
                user.DateOfBirth,
                user.Role.ToString()
            );
        }
}
