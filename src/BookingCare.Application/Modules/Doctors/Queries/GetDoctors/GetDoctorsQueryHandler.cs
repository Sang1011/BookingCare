using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Doctors.Queries.GetDoctors
{
    public class GetDoctorsQueryHandler(
        IDoctorRepository doctorRepository)
        : IRequestHandler<GetDoctorsQuery, Result<PagedResult<DoctorDto>>>
    {
        public async Task<Result<PagedResult<DoctorDto>>> Handle(
            GetDoctorsQuery request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await doctorRepository.GetPagedAsync(
                request.SearchName,
                request.SpecialtyId,
                request.AvailableDate,
                request.Page,
                request.PageSize,
                cancellationToken);

            return Result<PagedResult<DoctorDto>>.Success(
                new PagedResult<DoctorDto>(items, totalCount, request.Page, request.PageSize));
        }
    }
}
