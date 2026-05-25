using BookingCare.Application.Common.Interfaces;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordService _passwordService;
        private readonly ICurrentUser _currentUser;

        public ChangePasswordCommandHandler(
            IUserRepository userRepo,
            IUnitOfWork uow,
            IPasswordService passwordService,
            ICurrentUser currentUser)
        {
            _userRepo = userRepo;
            _uow = uow;
            _passwordService = passwordService;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            var user = await _userRepo.GetByIdAsync(_currentUser.UserId, ct);
            if (user is null)
                return Result.Failure(UserErrors.NotFound);

            if (!_passwordService.Verify(request.CurrentPassword, user.PasswordHash))
                return Result.Failure(UserErrors.InvalidCredentials);

            var newHash = _passwordService.Hash(request.NewPassword);
            user.ChangePassword(newHash);

            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
