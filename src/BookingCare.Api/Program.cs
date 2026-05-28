using BookingCare.Api.Endpoints;
using BookingCare.Api.Middleware;
using BookingCare.Application;
using BookingCare.Domain.Enums;
using BookingCare.Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) =>
    config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingCare API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(UserRole.Patient.ToString(), policy =>
        policy.RequireRole(UserRole.Patient.ToString()));

    options.AddPolicy(UserRole.Doctor.ToString(), policy =>
        policy.RequireRole(UserRole.Doctor.ToString()));

    options.AddPolicy(UserRole.Admin.ToString(), policy =>
        policy.RequireRole(UserRole.Admin.ToString()));

    options.AddPolicy(MultiRole.DoctorOrAdmin.ToString(), policy =>
        policy.RequireRole(UserRole.Doctor.ToString(), UserRole.Admin.ToString()));

    options.AddPolicy(MultiRole.PatientOrAdmin.ToString(), policy =>
        policy.RequireRole(UserRole.Patient.ToString(), UserRole.Admin.ToString()));

    options.AddPolicy(MultiRole.PatientOrDoctor.ToString(), policy =>
        policy.RequireRole(UserRole.Patient.ToString(), UserRole.Doctor.ToString()));
});

var app = builder.Build();
app.UseStaticFiles();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapDoctorEndpoints();
app.MapBookingEndpoints();
app.MapMedicalRecordEndpoints();
app.MapNotificationEndpoints();

app.Run();