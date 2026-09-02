using DuckDoku.Puzzle;

namespace DuckDoku.LevelBaker.Core;

public sealed record ProgressionBucket(int Size, PuzzleDifficulty Difficulty, int Count);

public static class ProgressionPlan
{
    public static IReadOnlyList<ProgressionBucket> Buckets { get; } = new[]
    {
        new ProgressionBucket(6, PuzzleDifficulty.Easy, 10),

        new ProgressionBucket(7, PuzzleDifficulty.Easy, 8),
        new ProgressionBucket(7, PuzzleDifficulty.Normal, 7),

        new ProgressionBucket(8, PuzzleDifficulty.Normal, 25),

        new ProgressionBucket(9, PuzzleDifficulty.Normal, 15),
        new ProgressionBucket(9, PuzzleDifficulty.Hard, 15),

        new ProgressionBucket(10, PuzzleDifficulty.Hard, 20),
    };
}
