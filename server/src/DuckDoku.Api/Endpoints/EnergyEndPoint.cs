using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api.Endpoints;

public record EnergyStateResponse(int energy, int energyMax, long energyRefillMs);

public static class EnergyEndPoint
{
    public static IEndpointRouteBuilder MapEnergyEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/v1/energy", GetEnergy);

        return builder;
    }

    private static async Task<IResult> GetEnergy(HttpContext context, AppDbContext database)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        EnergyState? energyState =
            await database.Energy.FirstOrDefaultAsync(energy => energy.PlayerId == device.PlayerId);

        if (energyState is null)
        {
            energyState = new EnergyState()
            {
                PlayerId = device.PlayerId,
                UpdatedAt = DateTime.UtcNow,
                Value = EnergyPolicy.Maximum,
            };

            await database.Energy.AddAsync(energyState);
            await database.SaveChangesAsync();
        }

        (int Value, TimeSpan TimeToNext) energyRefilMs = EnergyPolicy.GetCurrent(energyState.Value, energyState.UpdatedAt, DateTime.UtcNow);

        return Results.Ok(new EnergyStateResponse(energyState.Value, EnergyPolicy.Maximum, energyRefilMs.Value));
    }
}