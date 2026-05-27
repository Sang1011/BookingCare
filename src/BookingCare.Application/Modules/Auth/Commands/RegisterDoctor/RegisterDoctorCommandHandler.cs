using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Modules.Auth.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Auth;
using BookingCare.Domain.Entities.Doctor;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Auth.Commands.RegisterDoctor
{
    public class RegisterDoctorCommandHandler : IRequestHandler<RegisterDoctorCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordService _passwordService;

        public RegisterDoctorCommandHandler(
            IUserRepository userRepo,
            IDoctorRepository doctorRepo,
            IUnitOfWork uow,
            IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _doctorRepo = doctorRepo;
            _uow = uow;
            _passwordService = passwordService;
        }

        public async Task<Result<UserDto>> Handle(
            RegisterDoctorCommand request, CancellationToken ct)
        {
            var exists = await _userRepo.ExistsAsync(request.Email, ct);
            if (exists)
                return Result<UserDto>.Failure(UserErrors.EmailAlreadyExists);

            var passwordHash = _passwordService.Hash(request.Password);
            var userResult = User.Create(
                request.Email,
                passwordHash,
                request.FullName,
                request.Phone,
                request.DateOfBirth,
                UserRole.Doctor
            );

            if (userResult.IsFailure)
                return Result<UserDto>.Failure(userResult.Error!);

            var user = userResult.Value!;
            _userRepo.Add(user);

            var doctorResult = Doctor.Create(
                user.Id,
                request.SpecialtyId,
                request.LicenseNumber,
                request.YearsOfExperience,
                request.ConsultationFee,
                request.Bio
            );

            if (doctorResult.IsFailure)
                return Result<UserDto>.Failure(doctorResult.Error!);

            _doctorRepo.Add(doctorResult.Value!);

            await _uow.SaveChangesAsync(ct);

            return Result<UserDto>.Success(UserDto.FromEntity(user));
        }
    }
}