using DuckDoku.Puzzle;

namespace DuckDoku.LevelBaker.Core;

public static class LevelBaking
{
    private const int AttemptBudgetPerLevel = 2000;

    public static List<CatalogEntry> BakeBucket(ProgressionBucket bucket, PuzzleRandom random, ref int nextId)
    {
        var entries = new List<CatalogEntry>(bucket.Count);
        for (int i = 0; i < bucket.Count; i++)
        {
            entries.Add(BakeOne(bucket.Size, bucket.Difficulty, nextId, random));
            nextId++;
        }

        return entries;
    }

    private static CatalogEntry BakeOne(int size, PuzzleDifficulty difficulty, int id, PuzzleRandom random)
    {
        for (int attempt = 0; attempt < AttemptBudgetPerLevel; attempt++)
        {
            long seed = random.NextUInt();

            PuzzleDefinition definition;
            try
            {
                definition = PuzzleGenerator.Generate(size, difficulty, seed);
            }
            catch (InvalidOperationException)
            {
                continue;
            }

            SolveResult result = Solver.Solve(definition);
            if (result.Count != SolutionCount.Unique)
            {
                continue;
            }

            return new CatalogEntry
            {
                Id = id,
                Size = size,
                Seed = seed,
                Difficulty = difficulty,
                Regions = ExtractRegions(definition),
                Solution = result.Solution.ToArray(),
                BaseReward = RewardPolicy.GetRewardValue(size, difficulty),
            };
        }

        throw new InvalidOperationException(
            $"Failed to bake a unique {size}x{size} level ({difficulty}) within {AttemptBudgetPerLevel} attempts.");
    }

    private static int[] ExtractRegions(PuzzleDefinition definition)
    {
        int size = definition.Size;
        var regions = new int[size * size];

        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                regions[row * size + column] = definition.RegionAt(row, column);
            }
        }

        return regions;
    }
}
