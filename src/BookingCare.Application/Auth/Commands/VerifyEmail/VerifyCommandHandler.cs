using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _uow;
        private readonly IEmailVerificationService _verificationService;

        public VerifyEmailCommandHandler(
            IUserRepository userRepo,
            IUnitOfWork uow,
            IEmailVerificationService verificationService)
        {
            _userRepo = userRepo;
            _uow = uow;
            _verificationService = verificationService;
        }

        public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken ct)
        {
            var userId = await _verificationService.ValidateTokenAsync(request.Token, ct);
            if (userId is null)
                return Result.Failure(UserErrors.InvalidVerificationToken);

            var user = await _userRepo.GetByIdAsync(userId.Value, ct);
            if (user is null)
                return Result.Failure(UserErrors.NotFound);

            user.Activate();
            await _uow.SaveChangesAsync(ct);

            await _verificationService.RemoveTokenAsync(userId.Value, ct);

            return Result.Success();
        }
    }
}
