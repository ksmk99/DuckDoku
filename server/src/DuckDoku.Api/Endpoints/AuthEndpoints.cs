using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DuckDoku.Api.Endpoints
{
    public record GuestRequest(string DeviceId);

    public record GuestResponse(Guid PlayerId, string Token);

    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("/api/v1/auth/guest", GuestLogin);

            return builder;
        }

        private static async Task<IResult> GuestLogin(GuestRequest request, AppDbContext database)
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId))
            {
                return Results.BadRequest(new { error = "DeviceId is required." });
            }

            DateTime now = DateTime.UtcNow;

            Device? device = await database.Devices
                .FirstOrDefaultAsync(candidate => candidate.DeviceId == request.DeviceId);

            if (device is not null)
            {
                device.LastSeenAt = now;
                await database.SaveChangesAsync();

                return Results.Ok(new GuestResponse(device.PlayerId, device.Token));
            }

            Player player = new Player
            {
                Id = Guid.NewGuid(),
                CreatedAt = now
            };

            device = new Device
            {
                Id = Guid.NewGuid(),
                DeviceId = request.DeviceId,
                Token = PlayerAuthentication.CreateToken(),
                PlayerId = player.Id,
                LastSeenAt = now
            };

            database.Players.Add(player);
            database.Devices.Add(device);

            try
            {
                await database.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                if (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } postgres
                    && postgres.ConstraintName is not null
                    && postgres.ConstraintName.Contains("DeviceId", StringComparison.OrdinalIgnoreCase))
                {
                    database.Entry(player).State = EntityState.Detached;
                    database.Entry(device).State = EntityState.Detached;

                    device = await database.Devices
                        .FirstOrDefaultAsync(candidate => candidate.DeviceId == request.DeviceId);

                    if (device is null)
                    {
                        throw;
                    }
                }
                else
                {
                    throw; 
                }
            }

            return Results.Ok(new GuestResponse(device.PlayerId, device.Token));
        }
    }
}