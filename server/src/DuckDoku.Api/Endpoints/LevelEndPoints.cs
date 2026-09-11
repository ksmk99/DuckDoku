using DuckDoku.Contracts;
using DuckDoku.Puzzle;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DuckDoku.Api.Endpoints;

public record StartLevelResponse(Guid SessionId, int Energy, int EnergyMax, long EnergyRefillMs);

public record CompleteLevelResponse(long Duration, int Stars, int Balance, int CoinsEarned);

public record NextLevelResponse(int NextLevelId, int Energy, int EnergyMax, long EnergyRefillMs, LevelStars[] Stars);

public record LevelStars(int LevelId, int Stars);

public record CompleteLevelRequest(Guid SessionId, RequestCell[] placement);

public record RequestCell(int Row, int Column);

public record UseHintRequest(Guid SessionId);

public record UseHintResponse(int Hints);

public static class LevelEndpoints
{
    public static IEndpointRouteBuilder MapLevelEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/v1/levels/next", GetNextLevel);
        builder.MapPost("/api/v1/levels/{levelId}/start", StartLevel);
        builder.MapPost("/api/v1/levels/{levelId}/complete", CompleteLevel);
        builder.MapPost("/api/v1/levels/{levelId}/hint", UseHint);

        return builder;
    }

    private static async Task<IResult> GetNextLevel(HttpContext context,
        AppDbContext database,
        LevelCatalogService catalogService)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        LevelStars[] stars = await database.LevelProgress
            .Where(progress => progress.PlayerId == device.PlayerId)
            .Select(progress => new LevelStars(progress.LevelId, progress.BestResult))
            .ToArrayAsync();

        HashSet<int> completedSet = stars.Select(entry => entry.LevelId).ToHashSet();

        int nextLevelId = -1;
        foreach (var level in catalogService.Catalog.Levels.OrderBy(level => level.Id))
        {
            if (!completedSet.Contains(level.Id))
            {
                nextLevelId = level.Id;
                break;
            }
        }

        (int value, TimeSpan timeToNext) = await GetEnergyForDisplayAsync(database, device.PlayerId);

        return Results.Ok(new NextLevelResponse(nextLevelId, value, EnergyPolicy.Maximum,
            (long)timeToNext.TotalMilliseconds, stars));
    }

    private static async Task<(int Value, TimeSpan TimeToNext)> GetEnergyForDisplayAsync(AppDbContext database,
        Guid playerId)
    {
        EnergyState? energyState = await database.Energy
            .FirstOrDefaultAsync(energy => energy.PlayerId == playerId);

        int storedValue = energyState?.Value ?? EnergyPolicy.Maximum;

        return EnergyPolicy.GetCurrent(storedValue, energyState?.UpdatedAt, DateTime.UtcNow);
    }

    private static async Task<IResult> CompleteLevel(int levelId,
        CompleteLevelRequest request,
        HttpContext context,
        AppDbContext database,
        LevelCatalogService catalogService)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        if (!catalogService.Catalog.HasLevel(levelId))
        {
            throw new ApiException(ErrorCode.LevelLocked, "Level doesn't exist.");
        }

        var level = catalogService.Catalog.GetLevel(levelId);
        var definition = new PuzzleDefinition(level.Size, level.Seed, level.Difficulty, level.Regions, level.Solution);

        Cell[] placement = request.placement.Select(cell => new Cell(cell.Row, cell.Column)).ToArray();

        if (!PuzzleRules.IsSolved(definition, placement))
        {
            throw new ApiException(ErrorCode.InvalidSolution, "Level is not completed correctly.");
        }

        const int maxAttempts = 3;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var levelSession = await database.LevelSession
                .FirstOrDefaultAsync(session => session.Id == request.SessionId &&
                                                session.PlayerId == device.PlayerId &&
                                                session.LevelId == levelId);

            if (levelSession is null)
            {
                throw new ApiException(ErrorCode.LevelLocked, "Player didn't start this level.");
            }

            if (levelSession.ClaimedAt != null)
            {
                throw new ApiException(ErrorCode.SessionAlreadyClaimed, "Reward is already claimed.");
            }

            var playTime = DateTime.UtcNow - levelSession.StartsAt;
            var starsCount = StarPolicy.GetStars((int)level.Size, (long)playTime.TotalMilliseconds);

            levelSession.ClaimedAt = DateTime.UtcNow;

            LevelProgress? levelProgress = await database.LevelProgress
                .FirstOrDefaultAsync(progress =>
                    progress.LevelId == levelId &&
                    progress.PlayerId == device.PlayerId);

            if (levelProgress is null)
            {
                levelProgress = new LevelProgress
                {
                    Id = Guid.NewGuid(),
                    PlayerId = device.PlayerId,
                    LevelId = levelId,
                    BestResult = starsCount,
                    BestTimeMs = (long)playTime.TotalMilliseconds,
                    CompletedAt = DateTime.UtcNow
                };

                database.LevelProgress.Add(levelProgress);
            }
            else
            {
                levelProgress.BestResult = levelProgress.BestResult < starsCount ? starsCount : levelProgress.BestResult;
                levelProgress.BestTimeMs =
                    levelProgress.BestTimeMs > (long)playTime.TotalMilliseconds
                        ? (long)playTime.TotalMilliseconds
                        : levelProgress.BestTimeMs;
                levelProgress.CompletedAt = DateTime.UtcNow;
            }

            CurrencyState? currencyState = await database.Currency
                .FirstOrDefaultAsync(currency => currency.PlayerId == device.PlayerId);

            int balance = (currencyState?.Balance ?? 0) + level.BaseReward;

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

            try
            {
                await database.SaveChangesAsync();

                return Results.Ok(new CompleteLevelResponse((long)playTime.TotalMilliseconds, starsCount, balance,
                    level.BaseReward));
            }
            catch (DbUpdateConcurrencyException)
            {
                database.Entry(levelSession).State = EntityState.Detached;
                database.Entry(levelProgress).State = EntityState.Detached;
                database.Entry(currencyState).State = EntityState.Detached;
            }
            catch (DbUpdateException exception)
            {
                if (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
                {
                    database.Entry(levelSession).State = EntityState.Detached;
                    database.Entry(levelProgress).State = EntityState.Detached;
                    database.Entry(currencyState).State = EntityState.Detached;
                }
                else
                {
                    throw;
                }
            }
        }

        throw new ApiException(ErrorCode.VersionConflict, "Could not claim level reward, please retry.");
    }

    private static async Task<IResult> UseHint(int levelId,
        UseHintRequest request,
        HttpContext context,
        AppDbContext database,
        LevelCatalogService catalogService)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        if (!catalogService.Catalog.HasLevel(levelId))
        {
            throw new ApiException(ErrorCode.LevelLocked, "Level doesn't exist.");
        }

        var levelSession = await database.LevelSession
            .FirstOrDefaultAsync(session => session.Id == request.SessionId &&
                                            session.PlayerId == device.PlayerId &&
                                            session.LevelId == levelId);

        if (levelSession is null)
        {
            throw new ApiException(ErrorCode.LevelLocked, "Player didn't start this level.");
        }

        if (levelSession.ClaimedAt != null)
        {
            throw new ApiException(ErrorCode.SessionAlreadyClaimed, "Reward is already claimed.");
        }

        const int maxAttempts = 3;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            HintsState? hintsState = await database.Hints
                .FirstOrDefaultAsync(hints => hints.PlayerId == device.PlayerId);

            int count = hintsState?.Count ?? 0;

            if (count < 1)
            {
                throw new ApiException(ErrorCode.NotEnoughHints, "Not enough hints.");
            }

            count -= 1;

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

            try
            {
                await database.SaveChangesAsync();

                return Results.Ok(new UseHintResponse(count));
            }
            catch (DbUpdateConcurrencyException)
            {
                database.Entry(hintsState).State = EntityState.Detached;
            }
            catch (DbUpdateException exception)
            {
                if (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
                {
                    database.Entry(hintsState).State = EntityState.Detached;
                }
                else
                {
                    throw;
                }
            }
        }

        throw new ApiException(ErrorCode.VersionConflict, "Could not update hints, please retry.");
    }

    private static async Task<IResult> StartLevel(int levelId,
        HttpContext context,
        AppDbContext database,
        LevelCatalogService catalogService)
    {
        Device? device = await PlayerAuthentication.FindDeviceAsync(context, database);

        if (device is null)
        {
            return Results.Unauthorized();
        }

        if (!catalogService.Catalog.HasLevel(levelId))
        {
            throw new ApiException(ErrorCode.LevelLocked, "Level doesn't exist.");
        }

        if (levelId > 1)
        {
            var hadPreviousLevel = await database.LevelProgress
                .AnyAsync(level =>
                    level.LevelId == levelId - 1 && level.PlayerId == device.PlayerId);

            if (!hadPreviousLevel)
            {
                throw new ApiException(ErrorCode.LevelLocked, "Previous level is not completed.");
            }
        }

        const int maxAttempts = 3;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            EnergyState? energyState = await database.Energy
                .FirstOrDefaultAsync(energy => energy.PlayerId == device.PlayerId);

            int storedEnergy = energyState?.Value ?? EnergyPolicy.Maximum;

            bool canAfford = EnergyPolicy.TrySpend(storedEnergy, energyState?.UpdatedAt, DateTime.UtcNow,
                EnergyPolicy.EntryCost,
                out int newEnergyValue, out DateTime? newEnergyUpdatedAt);

            if (!canAfford)
            {
                throw new ApiException(ErrorCode.NotEnoughEnergy, "Not enough energy.");
            }

            LevelSession? levelSession = await database.LevelSession
                .FirstOrDefaultAsync(session => session.LevelId == levelId && session.PlayerId == device.PlayerId);

            if (levelSession is null)
            {
                levelSession = new LevelSession
                {
                    Id = Guid.NewGuid(),
                    LevelId = levelId,
                    PlayerId = device.PlayerId,
                    StartsAt = DateTime.UtcNow
                };

                database.LevelSession.Add(levelSession);
            }
            else
            {
                levelSession.StartsAt = DateTime.UtcNow;
                levelSession.ClaimedAt = null;
            }

            if (energyState is null)
            {
                energyState = new EnergyState
                {
                    PlayerId = device.PlayerId,
                    Value = newEnergyValue,
                    UpdatedAt = newEnergyUpdatedAt
                };

                database.Energy.Add(energyState);
            }
            else
            {
                energyState.Value = newEnergyValue;
                energyState.UpdatedAt = newEnergyUpdatedAt;
            }

            try
            {
                await database.SaveChangesAsync();

                (int _, TimeSpan timeToNext) = EnergyPolicy.GetCurrent(newEnergyValue, newEnergyUpdatedAt, DateTime.UtcNow);

                return Results.Ok(new StartLevelResponse(levelSession.Id, newEnergyValue, EnergyPolicy.Maximum,
                    (long)timeToNext.TotalMilliseconds));
            }
            catch (DbUpdateConcurrencyException)
            {
                database.Entry(energyState).State = EntityState.Detached;
                database.Entry(levelSession).State = EntityState.Detached;
            }
            catch (DbUpdateException exception)
            {
                if (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
                {
                    database.Entry(energyState).State = EntityState.Detached;
                    database.Entry(levelSession).State = EntityState.Detached;
                }
                else
                {
                    throw;
                }
            }
        }

        throw new ApiException(ErrorCode.VersionConflict, "Could not update energy, please retry.");
    }
}