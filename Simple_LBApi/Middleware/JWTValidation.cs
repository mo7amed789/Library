namespace Simple_LBApi.Middleware
{
    public sealed class JwtHeaderValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtHeaderValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var value = authHeader.ToString();
                if (!value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid authorization header format.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
