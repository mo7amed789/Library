using System.Net;
using System.Text.Json;
using Simple_LBApi.Common;

namespace Simple_LBApi.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, "Handled API error: {Message}", ex.Message);
                await WriteErrorAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled server error");
                await WriteErrorAsync(context, (int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = new ApiErrorResponse
            {
                Message = message,
                StatusCode = statusCode,
                TraceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
