using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Infrastructure.Email;
using BookingCare.Infrastructure.Persistence;
using BookingCare.Infrastructure.Persistence.Repositories;
using BookingCare.Worker.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog((services, config) =>
        config.ReadFrom.Configuration(builder.Configuration));

    // Database
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Repositories
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    // Email
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.AddScoped<IEmailService, MailKitEmailService>();

    // MassTransit + RabbitMQ
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<BookingConfirmedConsumer>();
        x.AddConsumer<BookingCancelledConsumer>();
        x.AddConsumer<BookingAutoExpiredConsumer>();
        x.AddConsumer<BookingRescheduledConsumer>();
        x.AddConsumer<DoctorPenaltyConsumer>();
        x.AddConsumer<DoctorScheduleCreatedConsumer>();
        x.AddConsumer<AccountLockedConsumer>();
        x.AddConsumer<SuspiciousLoginAttemptConsumer>();
        x.AddConsumer<EmailVerificationRequestedConsumer>();

        x.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
            {
                h.Username(builder.Configuration["RabbitMQ:Username"]!);
                h.Password(builder.Configuration["RabbitMQ:Password"]!);
            });

            cfg.ConfigureEndpoints(ctx);
        });
    });

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}