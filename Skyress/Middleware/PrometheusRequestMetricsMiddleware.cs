using Prometheus;

namespace Skyress.API.Middleware;

public sealed class PrometheusRequestMetricsMiddleware
{
    private static readonly Counter HttpRequestsTotal = Metrics.CreateCounter(
        "http_requests_total",
        "Total number of HTTP requests handled by the Skyress API.",
        new CounterConfiguration
        {
            LabelNames = ["namespace", "pod"]
        });

    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public PrometheusRequestMetricsMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        finally
        {
            var metricsPath = _configuration["Metrics:Path"] ?? "/metrics";
            if (!context.Request.Path.Equals(metricsPath, StringComparison.OrdinalIgnoreCase))
            {
                var namespaceName = Environment.GetEnvironmentVariable("POD_NAMESPACE") ?? "unknown";
                var podName = Environment.GetEnvironmentVariable("POD_NAME")
                    ?? Environment.GetEnvironmentVariable("HOSTNAME")
                    ?? "unknown";

                HttpRequestsTotal.WithLabels(namespaceName, podName).Inc();
            }
        }
    }
}
