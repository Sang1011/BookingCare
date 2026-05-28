namespace BookingCare.Application.Common.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
        Task SendToManyAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken ct = default);
    }
}