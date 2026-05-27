using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Domain.Events;
using MediatR;

namespace BookingCare.Application.Modules.Auth.EventHandlers
{
    public class SuspiciousLoginAttemptEventHandler
    : INotificationHandler<DomainEventNotification<SuspiciousLoginAttemptEvent>>
    {
        private readonly IEmailService _emailService;

        public SuspiciousLoginAttemptEventHandler(IEmailService emailService)
            => _emailService = emailService;

        public async Task Handle(
            DomainEventNotification<SuspiciousLoginAttemptEvent> notification, CancellationToken ct)
        {
            var e = notification.DomainEvent;
            await _emailService.SendAsync(
                e.Email,
                "⚠️ Cảnh báo bảo mật",
                "Có ai đó đang cố đăng nhập vào tài khoản của bạn. Nếu không phải bạn hãy đổi mật khẩu ngay.",
                ct);
        }
    }
}
