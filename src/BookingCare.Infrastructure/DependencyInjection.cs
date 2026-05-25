using BookingCare.Application.Common.Interfaces;
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
using Microsoft.IdentityModel.Tokens;

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

            // Auth
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<ITokenService, JwtService>();
            services.AddScoped<IPasswordService, PasswordService>();

            // Redis
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
            services.AddScoped<ILoginAttemptService, RedisLoginAttemptService>();

            // Email
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, MailKitEmailService>();

            // CurrentUser
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUserService>();

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

            return services;
        }
    }
}
