using FluentValidation;

namespace BookingCare.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                await context.Response.WriteAsJsonAsync(new
                {
                    type = "ValidationError",
                    errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    type = "ServerError",
                    message = "Đã có lỗi xảy ra, vui lòng thử lại sau"
                });
            }
        }
    }
}