using DuckDoku.Puzzle;

namespace DuckDoku.LevelBaker.Core;

public sealed record CatalogEntry
{
    public required int Id { get; init; }
    public required int Size { get; init; }
    public required long Seed { get; init; }
    public required PuzzleDifficulty Difficulty { get; init; }
    public required int[] Regions { get; init; }
    public required Cell[] Solution { get; init; }
    public required int BaseReward { get; init; }
}
