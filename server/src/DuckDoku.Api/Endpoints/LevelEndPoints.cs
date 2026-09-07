using DuckDoku.Contracts;
using DuckDoku.Puzzle;
using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api.Endpoints;

public record StartLevelResponse(Guid SessionId, int Energy, int EnergyMax, long EnergyRefillMs);

public record CompleteLevelResponse(long Duration, int Stars);

public record NextLevelResponse(int NextLevelId, int Energy, int EnergyMax, long EnergyRefillMs);

public record CompleteLevelRequest(Guid SessionId, RequestCell[] placement);

public record RequestCell(int Row, int Column);

public static class LevelEndpoints
{
    public static IEndpointRouteBuilder MapLevelEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/v1/levels/next", GetNextLevel);
        builder.MapPost("/api/v1/levels/{levelId}/start", StartLevel);
        builder.MapPost("/api/v1/levels/{levelId}/complete", CompleteLevel);

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

        List<int> completedLevelIds = await database.LevelProgress
            .Where(progress => progress.PlayerId == device.PlayerId)
            .Select(progress => progress.LevelId)
            .ToListAsync();

        HashSet<int> completedSet = completedLevelIds.ToHashSet();

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

        return Results.Ok(new NextLevelResponse(nextLevelId, value, EnergyPolicy.Maximum, (long)timeToNext.TotalMilliseconds));
    }

    private static async Task<(int Value, TimeSpan TimeToNext)> GetEnergyForDisplayAsync(AppDbContext database, Guid playerId)
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

        var level = catalogService.Catalog.GetLevel(levelId);
        var definition = new PuzzleDefinition(level.Size, level.Seed, level.Difficulty, level.Regions, level.Solution);

        Cell[] placement = request.placement.Select(cell => new Cell(cell.Row, cell.Column)).ToArray();

        if (!PuzzleRules.IsSolved(definition, placement))
        {
            throw new ApiException(ErrorCode.InvalidSolution, "Level is not completed correctly.");
        }

        var playTime = DateTime.UtcNow - levelSession.StartsAt;
        var starsCount = StarPolicy.GetStars((int)level.Size, (long)playTime.TotalMilliseconds);

        levelSession.ClaimedAt = DateTime.UtcNow;

        LevelProgress? levelProgress = await database.LevelProgress
            .FirstOrDefaultAsync(level =>
                level.LevelId == levelId &&
                level.PlayerId == device.PlayerId);

        if (levelProgress is null)
        {
            levelProgress = new LevelProgress()
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

        await database.SaveChangesAsync();

        return Results.Ok(new CompleteLevelResponse((long)playTime.TotalMilliseconds, starsCount));
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

        EnergyState? energyState = await database.Energy
            .FirstOrDefaultAsync(energy => energy.PlayerId == device.PlayerId);

        int storedEnergy = energyState?.Value ?? EnergyPolicy.Maximum;

        bool canAfford = EnergyPolicy.TrySpend(storedEnergy, energyState?.UpdatedAt, DateTime.UtcNow, EnergyPolicy.EntryCost,
            out int newEnergyValue, out DateTime? newEnergyUpdatedAt);

        if (!canAfford)
        {
            throw new ApiException(ErrorCode.NotEnoughEnergy, "Not enough energy.");
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

        var levelSession = await database.LevelSession
            .FirstOrDefaultAsync(session => session.LevelId == levelId && session.PlayerId == device.PlayerId);

        if (levelSession is null)
        {
            levelSession = new LevelSession()
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
        }

        if (energyState is null)
        {
            database.Energy.Add(new EnergyState()
            {
                PlayerId = device.PlayerId,
                Value = newEnergyValue,
                UpdatedAt = newEnergyUpdatedAt
            });
        }
        else
        {
            energyState.Value = newEnergyValue;
            energyState.UpdatedAt = newEnergyUpdatedAt;
        }

        await database.SaveChangesAsync();

        (int _, TimeSpan timeToNext) = EnergyPolicy.GetCurrent(newEnergyValue, newEnergyUpdatedAt, DateTime.UtcNow);

        return Results.Ok(new StartLevelResponse(levelSession.Id, newEnergyValue, EnergyPolicy.Maximum, (long)timeToNext.TotalMilliseconds));
    }
}