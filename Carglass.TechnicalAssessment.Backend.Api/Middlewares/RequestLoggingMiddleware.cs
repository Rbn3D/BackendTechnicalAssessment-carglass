using System.Diagnostics;

namespace Carglass.TechnicalAssessment.Backend.Api.Middlewares;

public sealed class RequestLoggingMiddleware
{
    private static readonly PathString HealthCheckPath = "/HealthCheck";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["TraceId"] = context.TraceIdentifier
        });

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var level = context.Request.Path.StartsWithSegments(HealthCheckPath)
                ? LogLevel.Debug
                : LogLevel.Information;

            _logger.Log(level, "HTTP {Method} {Path} respondió {StatusCode} en {ElapsedMs} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
