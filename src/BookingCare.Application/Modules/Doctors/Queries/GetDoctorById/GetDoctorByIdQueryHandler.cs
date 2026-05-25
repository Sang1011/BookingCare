using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using MediatR;

namespace BookingCare.Application.Modules.Doctors.Queries.GetDoctorById
{
    public class GetDoctorByIdQueryHandler(IDoctorRepository doctorRepository)
        : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>
    {
        public async Task<Result<DoctorDto>> Handle(
            GetDoctorByIdQuery request,
            CancellationToken cancellationToken)
        {
            var doctor = await doctorRepository.GetByIdWithSpecialtyAsync(
                request.DoctorId, cancellationToken);

            if (doctor is null)
                return Result<DoctorDto>.Failure(DoctorErrors.NotFound);

            var dto = new DoctorDto(
                doctor.Id,
                doctor.User.FullName,
                doctor.User.Email,
                doctor.Specialty.Name,
                doctor.LicenseNumber,
                doctor.YearsOfExperience,
                doctor.ConsultationFee,
                doctor.Bio,
                doctor.AvatarUrl,
                doctor.IsVerified);

            return Result<DoctorDto>.Success(dto);
        }
    }
}
