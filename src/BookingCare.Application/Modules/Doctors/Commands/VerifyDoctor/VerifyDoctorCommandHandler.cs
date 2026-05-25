using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.Commands.VerifyDoctor
{
    public class VerifyDoctorCommandHandler(
    IDoctorRepository doctorRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<VerifyDoctorCommand, Result>
    {
        public async Task<Result> Handle(
            VerifyDoctorCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
            if (doctor is null)
                return Result.Failure(DoctorErrors.NotFound);

            if (doctor.IsVerified)
                return Result.Failure(DoctorErrors.AlreadyVerified);

            doctor.Verify();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
