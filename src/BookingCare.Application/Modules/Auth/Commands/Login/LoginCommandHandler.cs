using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using BookingCare.Domain.Events;
using MediatR;
using RefreshTokenEntity = BookingCare.Domain.Entities.Auth.RefreshToken;

namespace BookingCare.Application.Modules.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenResponse>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly ILoginAttemptService _loginAttemptService;
        private readonly IPublisher _publisher;

        public LoginCommandHandler(
            IUserRepository userRepo,
            IRefreshTokenRepository refreshTokenRepo,
            IUnitOfWork uow,
            IPasswordService passwordService,
            ITokenService tokenService,
            ILoginAttemptService loginAttemptService,
            IPublisher publisher)
        {
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _uow = uow;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _loginAttemptService = loginAttemptService;
            _publisher = publisher;
        }

        public async Task<Result<TokenResponse>> Handle(
            LoginCommand request, CancellationToken ct)
        {
            var isLocked = await _loginAttemptService.IsLockedAsync(request.Email, ct);
            if (isLocked)
                return Result<TokenResponse>.Failure(UserErrors.AccountLocked);

            var user = await _userRepo.GetByEmailAsync(request.Email, ct);
            if (user is null || !user.IsActive)
            {
                await _loginAttemptService.IncrementAsync(request.Email, ct);
                return Result<TokenResponse>.Failure(UserErrors.InvalidCredentials);
            }

            if (!_passwordService.Verify(request.Password, user.PasswordHash))
            {
                var attempts = await _loginAttemptService.IncrementAsync(request.Email, ct);

                if (attempts == 4)
                    await _publisher.Publish(
                        new SuspiciousLoginAttemptEvent(user.Id, user.Email), ct);

                if (attempts >= 5)
                    await _publisher.Publish(
                        new AccountLockedEvent(user.Id, user.Email,
                            DateTime.UtcNow.AddMinutes(15)), ct);

                return Result<TokenResponse>.Failure(UserErrors.InvalidCredentials);
            }

            await _loginAttemptService.ResetAsync(request.Email, ct);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var rawRefreshToken = _tokenService.GenerateRefreshToken();
            var hashedRefreshToken = _passwordService.Hash(rawRefreshToken);

            var refreshToken = RefreshTokenEntity.Create(user.Id, hashedRefreshToken);
            _refreshTokenRepo.Add(refreshToken);
            await _uow.SaveChangesAsync(ct);

            return Result<TokenResponse>.Success(new TokenResponse(
                accessToken,
                rawRefreshToken,
                DateTime.UtcNow.AddMinutes(15)
            ));
        }
    }
}
