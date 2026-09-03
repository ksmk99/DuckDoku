using DuckDoku.Puzzle;

namespace DuckDoku.Api;

public struct LevelRecord
{
    public int Id { get; init; }
    public int Size { get; init; }
    public long Seed { get; init; }
    public PuzzleDifficulty Difficulty { get; init; }
    public int[] Regions { get; init; }
    public Cell[] Solution { get; init; }
    public int BaseReward { get; init; }
}