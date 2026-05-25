using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Application.Modules.Auth.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
    {
        private readonly IUserRepository _userRepo;
        private readonly ICurrentUser _currentUser;

        public GetCurrentUserQueryHandler(IUserRepository userRepo, ICurrentUser currentUser)
        {
            _userRepo = userRepo;
            _currentUser = currentUser;
        }

        public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken ct)
        {
            var user = await _userRepo.GetByIdAsync(_currentUser.UserId, ct);
            if (user is null)
                return Result<UserDto>.Failure(UserErrors.NotFound);

            return Result<UserDto>.Success(UserDto.FromEntity(user));
        }
    }

}
