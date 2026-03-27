namespace Simple_LBApi.Middleware
{
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startedAt = DateTime.UtcNow;
            await _next(context);
            var durationMs = (DateTime.UtcNow - startedAt).TotalMilliseconds;

            _logger.LogInformation(
                "HTTP {Method} {Path} -> {StatusCode} in {DurationMs}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                Math.Round(durationMs, 2));
        }
    }
}
