using DuckDoku.Puzzle;

namespace DuckDoku.LevelBaker.Core;

public static class RewardPolicy
{
    public static int GetRewardValue(int size, PuzzleDifficulty difficulty)
    {
        int difficultyMultiplier = 1 << (int)difficulty;
        int raw = size * difficultyMultiplier;

        return (int)Math.Round(raw / 5.0, MidpointRounding.AwayFromZero) * 5;
    }
}