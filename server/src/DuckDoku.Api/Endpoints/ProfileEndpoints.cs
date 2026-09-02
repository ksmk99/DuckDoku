using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api.Endpoints
{
    public record ProfileResponse(Guid PlayerId, string? DisplayName, DateTime CreatedAt, DateTime LastSeenAt);

    public record ChangeNameRequest(string DisplayName);

    public static class ProfileEndpoints
    {
        private const int MaxDisplayNameLength = 32;

        public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/api/v1/profile", GetProfile);
            builder.MapPut("/api/v1/profile/name", ChangeName);

            return builder;
        }

        private static async Task<IResult> GetProfile(HttpContext context, AppDbContext database)
        {
            Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

            if (device is null)
            {
                return Results.Unauthorized();
            }

            Player? player = await database.Players
                .FirstOrDefaultAsync(candidate => candidate.Id == device.PlayerId);

            if (player is null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(ToResponse(player, device));
        }

        private static async Task<IResult> ChangeName(ChangeNameRequest request, HttpContext context,
            AppDbContext database)
        {
            Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

            if (device is null)
            {
                return Results.Unauthorized();
            }

            string name = request.DisplayName.Trim() ?? "";

            if (name.Length == 0 || name.Length > MaxDisplayNameLength)
            {
                return Results.BadRequest(new { error = $"DisplayName must be 1..{MaxDisplayNameLength} characters." });
            }

            Player? player = await database.Players
                .FirstOrDefaultAsync(candidate => candidate.Id == device.PlayerId);

            if (player is null)
            {
                return Results.Unauthorized();
            }

            player.DisplayName = name;

            await database.SaveChangesAsync();

            return Results.Ok(ToResponse(player, device));
        }

        private static ProfileResponse ToResponse(Player player, Device device)
        {
            return new ProfileResponse(player.Id, player.DisplayName, player.CreatedAt, device.LastSeenAt);
        }
    }
}