using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _currentUser;

        public LogoutCommandHandler(
            IRefreshTokenRepository refreshTokenRepo,
            IUnitOfWork uow,
            ICurrentUser currentUser)
        {
            _refreshTokenRepo = refreshTokenRepo;
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
        {
            await _refreshTokenRepo.RevokeAllByUserIdAsync(_currentUser.UserId, ct);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
