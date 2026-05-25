using BookingCare.Application.Common.Interfaces;
using BookingCare.Application.Common.Models;
using BookingCare.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Auth.EventHandlers
{
    public class AccountLockedEventHandler
    : INotificationHandler<DomainEventNotification<AccountLockedEvent>>
    {
        private readonly IEmailService _emailService;

        public AccountLockedEventHandler(IEmailService emailService)
            => _emailService = emailService;

        public async Task Handle(
            DomainEventNotification<AccountLockedEvent> notification, CancellationToken ct)
        {
            var e = notification.Event;
            await _emailService.SendAsync(
                e.Email,
                "🔒 Tài khoản bị tạm khóa",
                $"Tài khoản bị khóa đến {e.LockedUntil:HH:mm dd/MM/yyyy}. Nếu không phải bạn hãy liên hệ hỗ trợ ngay.",
                ct);
        }
    }

}
