using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api.Endpoints;

public sealed record DevEnergyResponse(int Energy, int EnergyMax, long EnergyRefillMs);

public static class DevEndpoints
{
    public static IEndpointRouteBuilder MapDevEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/v1/dev/energy/max", GrantMaxEnergy);

        return builder;
    }

    private static async Task<IResult> GrantMaxEnergy(HttpContext context, AppDbContext database)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        EnergyState? energyState = await database.Energy
            .FirstOrDefaultAsync(energy => energy.PlayerId == device.PlayerId);

        if (energyState is null)
        {
            database.Energy.Add(new EnergyState()
            {
                PlayerId = device.PlayerId,
                Value = EnergyPolicy.Maximum,
                UpdatedAt = null
            });
        }
        else
        {
            energyState.Value = EnergyPolicy.Maximum;
            energyState.UpdatedAt = null;
        }

        await database.SaveChangesAsync();

        return Results.Ok(new DevEnergyResponse(EnergyPolicy.Maximum, EnergyPolicy.Maximum, 0));
    }
}
