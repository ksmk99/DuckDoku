using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api.Endpoints;

public sealed record DevEnergyResponse(int Energy, int EnergyMax, long EnergyRefillMs);

public sealed record DevCurrencyResponse(int Balance);

public static class DevEndpoints
{
    private const int DevGrantAmount = 1000;

    public static IEndpointRouteBuilder MapDevEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/v1/dev/energy/max", GrantMaxEnergy);
        builder.MapPost("/api/v1/dev/currency/grant", GrantCurrency);

        return builder;
    }

    private static async Task<IResult> GrantCurrency(HttpContext context, AppDbContext database)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        CurrencyState? currencyState = await database.Currency
            .FirstOrDefaultAsync(currency => currency.PlayerId == device.PlayerId);

        int balance = (currencyState?.Balance ?? 0) + DevGrantAmount;

        if (currencyState is null)
        {
            database.Currency.Add(new CurrencyState()
            {
                PlayerId = device.PlayerId,
                Balance = balance
            });
        }
        else
        {
            currencyState.Balance = balance;
        }

        await database.SaveChangesAsync();

        return Results.Ok(new DevCurrencyResponse(balance));
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
