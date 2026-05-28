using BookingCare.Infrastructure.Persistence;
using BookingCare.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using System.Text;
using BookingCare.Infrastructure.Auth;
using BookingCare.Infrastructure.Cache;
using BookingCare.Infrastructure.Email;
using BookingCare.Infrastructure.Storage;
using Microsoft.IdentityModel.Tokens;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Security;
using BookingCare.Infrastructure.Messaging;
using MassTransit;
using Hangfire;
using Hangfire.Redis.StackExchange;

namespace BookingCare.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // UnitOfWork & Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
            services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();

            // Auth
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<ITokenService, JwtService>();
            services.AddScoped<IPasswordService, PasswordService>();

            // Redis
            var redisConnection = ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("Redis")!);
            services.AddSingleton<IConnectionMultiplexer>(redisConnection);
            services.AddScoped<ILoginAttemptService, RedisLoginAttemptService>();
            services.AddScoped<IEmailVerificationService, RedisEmailVerificationService>();

            // Email
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, MailKitEmailService>();

            // CurrentUser
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUserService>();

            // File Storage
            services.Configure<AzureBlobStorageSettings>(
                configuration.GetSection(AzureBlobStorageSettings.SectionName));

            var storageProvider = configuration["FileStorage:Provider"];
            if (storageProvider == "AzureBlob")
                services.AddScoped<IFileStorageService, AzureBlobStorageService>();
            else
                services.AddScoped<IFileStorageService, LocalFileStorageService>();

            // JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            // MassTransit + RabbitMQ
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]!);
                        h.Password(configuration["RabbitMQ:Password"]!);
                    });
                });
            });
            services.AddScoped<IMessagePublisher, MassTransitMessagePublisher>();

            // Hangfire + Redis
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(redisConnection, new RedisStorageOptions
                {
                    Prefix = "hangfire:",
                    Db = 1
                }));
            services.AddHangfireServer();

            return services;
        }
    }
}