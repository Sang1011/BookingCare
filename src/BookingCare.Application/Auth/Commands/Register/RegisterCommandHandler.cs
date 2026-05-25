using BookingCare.Application.Auth.DTOs;
using BookingCare.Application.Common.Interfaces;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Auth;
using BookingCare.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordService _passwordService;

        public RegisterCommandHandler(
            IUserRepository userRepo,
            IUnitOfWork uow,
            IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _uow = uow;
            _passwordService = passwordService;
        }

        public async Task<Result<UserDto>> Handle(
            RegisterCommand request, CancellationToken ct)
        {
            var exists = await _userRepo.ExistsAsync(request.Email, ct);
            if (exists)
                return Result<UserDto>.Failure(UserErrors.EmailAlreadyExists);

            var passwordHash = _passwordService.Hash(request.Password);

            var user = User.Create(
                request.Email,
                passwordHash,
                request.FullName,
                request.Phone,
                request.DateOfBirth
            );

            _userRepo.Add(user);
            await _uow.SaveChangesAsync(ct);

            return Result<UserDto>.Success(UserDto.FromEntity(user));
        }
    }
}
