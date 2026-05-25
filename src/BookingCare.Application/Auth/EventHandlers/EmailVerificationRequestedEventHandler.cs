using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Domain.Events;
using MediatR;

namespace BookingCare.Application.Auth.EventHandlers
{
    public class EmailVerificationRequestedEventHandler
        : INotificationHandler<DomainEventNotification<EmailVerificationRequestedEvent>>
    {
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IEmailService _emailService;

        public EmailVerificationRequestedEventHandler(
            IEmailVerificationService emailVerificationService,
            IEmailService emailService)
        {
            _emailVerificationService = emailVerificationService;
            _emailService = emailService;
        }

        public async Task Handle(
            DomainEventNotification<EmailVerificationRequestedEvent> notification,
            CancellationToken ct)
        {
            var e = notification.Event;

            var token = await _emailVerificationService.GenerateTokenAsync(e.UserId, ct);

            await _emailService.SendAsync(
                e.Email,
                "Xác nhận tài khoản BookingCare",
                $"""
                <h2>Xin chào!</h2>
                <p>Cảm ơn bạn đã đăng ký tài khoản BookingCare.</p>
                <p>Vui lòng xác nhận email bằng cách bấm vào link bên dưới:</p>
                <a href="http://localhost:5273/api/v1/auth/verify-email?token={token}">
                    Xác nhận email
                </a>
                <p>Link có hiệu lực trong 24 giờ.</p>
                """,
                ct);
        }
    }
}