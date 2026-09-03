using DuckDoku.Contracts;
using DuckDoku.Puzzle;
using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api.Endpoints;

public record StartLevelResponse(Guid SessionId);
public record CompleteLevelResponse(long Duration, int Stars);
public record NextLevelResponse(int NextLevelId);

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

        return Results.Ok(new NextLevelResponse(nextLevelId));
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
                LevelId =  levelId,
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
                levelProgress.BestTimeMs > (long)playTime.TotalMilliseconds ? (long)playTime.TotalMilliseconds : levelProgress.BestTimeMs;
            levelProgress.CompletedAt = DateTime.UtcNow;
        }

        await database.SaveChangesAsync();
        
        return  Results.Ok(new CompleteLevelResponse((long)playTime.TotalMilliseconds, starsCount));
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
            throw new ApiException(ErrorCode.LevelLocked, "Previous level is not completed.");
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

            await database.SaveChangesAsync();
        }
        else
        {
            levelSession.StartsAt = DateTime.UtcNow;
            await database.SaveChangesAsync();
        }

        return Results.Ok(new StartLevelResponse(levelSession.Id));
    }
}