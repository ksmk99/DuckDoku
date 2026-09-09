using DuckDoku.Contracts;

namespace DuckDoku.Api.Endpoints;

public record CurrencyStateResponse(int balance);

public static class CurrencyEndPoint
{
    public static IEndpointRouteBuilder MapCurrenciesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/v1/currency", GetCurrency);

        return builder;
    }

    private static async Task<IResult> GetCurrency(HttpContext context, AppDbContext database)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        CurrencyState? currency = database.Currency.FirstOrDefault(currency => currency.PlayerId == device.PlayerId);
        if (currency is null)
        {
            currency = new CurrencyState
            {
                PlayerId = device.PlayerId,
                Balance = 0
            };
            
            await database.Currency.AddAsync(currency);
            await database.SaveChangesAsync();
        }
        
        return Results.Ok(new CurrencyStateResponse(currency.Balance));
    }
}