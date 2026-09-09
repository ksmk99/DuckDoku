using DuckDoku.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api.Endpoints;

public record HintsStateResponse(int Hints);

public record PurchaseHintResponse(int Balance, int Hints);

public static class HintsEndPoint
{
    public static IEndpointRouteBuilder MapHintsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/v1/hints", GetHints);
        builder.MapPost("/api/v1/hints/purchase", PurchaseHint);

        return builder;
    }

    private static async Task<IResult> GetHints(HttpContext context, AppDbContext database)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        HintsState? hintsState = await database.Hints
            .FirstOrDefaultAsync(hints => hints.PlayerId == device.PlayerId);

        return Results.Ok(new HintsStateResponse(hintsState?.Count ?? 0));
    }

    private static async Task<IResult> PurchaseHint(HttpContext context, AppDbContext database)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        CurrencyState? currencyState = await database.Currency
            .FirstOrDefaultAsync(currency => currency.PlayerId == device.PlayerId);

        int balance = currencyState?.Balance ?? 0;

        if (balance < HintPolicy.Cost)
        {
            throw new ApiException(ErrorCode.NotEnoughCurrency, "Not enough currency.");
        }

        balance -= HintPolicy.Cost;

        if (currencyState is null)
        {
            database.Currency.Add(new CurrencyState
            {
                PlayerId = device.PlayerId,
                Balance = balance
            });
        }
        else
        {
            currencyState.Balance = balance;
        }

        HintsState? hintsState = await database.Hints
            .FirstOrDefaultAsync(hints => hints.PlayerId == device.PlayerId);

        int count = (hintsState?.Count ?? 0) + 1;

        if (hintsState is null)
        {
            database.Hints.Add(new HintsState
            {
                PlayerId = device.PlayerId,
                Count = count
            });
        }
        else
        {
            hintsState.Count = count;
        }

        await database.SaveChangesAsync();

        return Results.Ok(new PurchaseHintResponse(balance, count));
    }
}
