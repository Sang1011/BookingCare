using BookingCare.Application.Auth.DTOs;
using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Auth;
using BookingCare.Domain.Entities.Doctor;
using BookingCare.Domain.Enums;
using BookingCare.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.Commands.RegisterDoctor
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
            var user = User.Create(
                request.Email,
                passwordHash,
                request.FullName,
                request.Phone,
                request.DateOfBirth,
                UserRole.Doctor
            );
            _userRepo.Add(user);

            var doctorResult = Doctor.Create(
                user.Id,
                request.SpecialtyId,
                request.LicenseNumber,
                request.YearsOfExperience,
                request.ConsultationFee,
                request.Bio
            );
            _doctorRepo.Add(doctorResult);

            await _uow.SaveChangesAsync(ct);

            return Result<UserDto>.Success(UserDto.FromEntity(user));
        }
    }
}
