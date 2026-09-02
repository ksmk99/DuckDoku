namespace DuckDoku.Api.Endpoints
{
    public sealed record ServerTimeResponse(long UnixTimeMs);

    public static class ServerTimeEndPoints
    {
        public static IEndpointRouteBuilder MapServerTimeEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/api/v1/time", GetServerTime);

            return builder;
        }

        private static async Task<IResult> GetServerTime(HttpContext context, AppDbContext database)
        {
            Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

            if (device is null)
            {
                return Results.Unauthorized();
            }

            long unixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            return Results.Ok(new ServerTimeResponse(unixTime));
        }
    }
}
