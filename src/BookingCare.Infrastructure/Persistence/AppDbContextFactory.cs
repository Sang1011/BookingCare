using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MediatR;

namespace BookingCare.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BookingCare.Api"))
            .AddJsonFile("appsettings.json")
            .Build();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .Options;

        return new AppDbContext(options, new NoOpPublisher());
    }
}

public class NoOpPublisher : IPublisher
{
    public Task Publish(object notification, CancellationToken ct = default) => Task.CompletedTask;
    public Task Publish<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification => Task.CompletedTask;
}