using BookingCare.Application.Common.Models;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using RefreshTokenEntity = BookingCare.Domain.Entities.Auth.RefreshToken;
using MediatR;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Interfaces.Persistence;

namespace BookingCare.Application.Modules.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public RefreshTokenCommandHandler(
            IUserRepository userRepo,
            IRefreshTokenRepository refreshTokenRepo,
            IUnitOfWork uow,
            ITokenService tokenService,
            IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _uow = uow;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<Result<TokenResponse>> Handle(
            RefreshTokenCommand request, CancellationToken ct)
        {
            var userId = _tokenService.GetUserIdFromExpiredToken(request.AccessToken);
            if (userId is null)
                return Result<TokenResponse>.Failure(UserErrors.InvalidCredentials);

            var hashedToken = _passwordService.Hash(request.RefreshToken);
            var refreshToken = await _refreshTokenRepo.GetByTokenAsync(hashedToken, ct);

            if (refreshToken is null || !refreshToken.IsValid() || refreshToken.UserId != userId)
                return Result<TokenResponse>.Failure(UserErrors.InvalidCredentials);

            var user = await _userRepo.GetByIdAsync(userId.Value, ct);
            if (user is null || !user.IsActive)
                return Result<TokenResponse>.Failure(UserErrors.InvalidCredentials);

            refreshToken.Revoke();

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRawRefreshToken = _tokenService.GenerateRefreshToken();
            var newHashedRefreshToken = _passwordService.Hash(newRawRefreshToken);

            var newRefreshToken = RefreshTokenEntity.Create(user.Id, newRawRefreshToken);
            _refreshTokenRepo.Add(newRefreshToken);
            await _uow.SaveChangesAsync(ct);

            return Result<TokenResponse>.Success(new TokenResponse(
                newAccessToken,
                newRawRefreshToken,
                DateTime.UtcNow.AddMinutes(15)
            ));
        }
    }
}
