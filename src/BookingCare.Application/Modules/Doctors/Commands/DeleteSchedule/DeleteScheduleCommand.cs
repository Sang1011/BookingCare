using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Modules.Doctors.Commands.DeleteSchedule
{
    public record DeleteScheduleCommand(Guid ScheduleId) : IRequest<Result>;
}
