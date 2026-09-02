using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DuckDoku.Api.Endpoints
{
    public static class HealthEndpoints
    {
        public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapHealthChecks("/api/v1/health", new HealthCheckOptions()
            {
                ResponseWriter = WriteStatus
            });
            
            builder.MapGet("/api/v1/ready", CheckReady);
            
            return builder;
        }

        private static async Task<IResult> CheckReady(AppDbContext database)
        {
            bool canConnect = await database.Database.CanConnectAsync();

            if (!canConnect)
            {
                return Results.Json(
                    new { status = "unavailable" },
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            return Results.Ok(new { status = "ready" });
        }
        
        private static Task WriteStatus(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            
            return context.Response.WriteAsJsonAsync(new { status = report.Status.ToString() });
        }
    }
}