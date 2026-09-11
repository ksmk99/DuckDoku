using System.Text.Json;
using DuckDoku.Contracts;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DuckDoku.Api.Endpoints;

public record HintsStateResponse(int Hints);

public record PurchaseHintResponse(int Balance, int Hints);

public record PurchaseHintRequest(Guid RequestId);

public static class HintsEndPoint
{
    private static readonly JsonSerializerOptions ResponseJsonOptions = new(JsonSerializerDefaults.Web);

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

    private static async Task<IResult> PurchaseHint(PurchaseHintRequest request, HttpContext context,
        AppDbContext database)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        IResult? replay = await IdempotencyGuard.FindReplayAsync(database, device.PlayerId, request.RequestId);

        if (replay is not null)
        {
            return replay;
        }

        const int maxAttempts = 3;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
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
                currencyState = new CurrencyState
                {
                    PlayerId = device.PlayerId,
                    Balance = balance
                };

                database.Currency.Add(currencyState);
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
                hintsState = new HintsState
                {
                    PlayerId = device.PlayerId,
                    Count = count
                };

                database.Hints.Add(hintsState);
            }
            else
            {
                hintsState.Count = count;
            }

            var response = new PurchaseHintResponse(balance, count);
            var record = new IdempotencyRecord
            {
                RequestId = request.RequestId,
                PlayerId = device.PlayerId,
                ResponseBody = JsonSerializer.Serialize(response, ResponseJsonOptions),
                CreatedAt = DateTime.UtcNow
            };

            database.IdempotencyRecords.Add(record);

            try
            {
                await database.SaveChangesAsync();

                return Results.Ok(response);
            }
            catch (DbUpdateConcurrencyException)
            {
                database.Entry(currencyState).State = EntityState.Detached;
                database.Entry(hintsState).State = EntityState.Detached;
                database.Entry(record).State = EntityState.Detached;
            }
            catch (DbUpdateException exception)
            {
                if (exception.InnerException is not PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
                {
                    throw;
                }

                database.Entry(currencyState).State = EntityState.Detached;
                database.Entry(hintsState).State = EntityState.Detached;
                database.Entry(record).State = EntityState.Detached;

                if (IdempotencyGuard.IsConflict(exception))
                {
                    IResult? conflictReplay = await IdempotencyGuard.FindReplayAsync(database, device.PlayerId, request.RequestId);

                    if (conflictReplay is not null)
                    {
                        return conflictReplay;
                    }
                }
            }
        }

        throw new ApiException(ErrorCode.VersionConflict, "Could not update currency or hints, please retry.");
    }
}