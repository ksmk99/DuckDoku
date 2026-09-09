using System;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class LevelFinishContext : ILevelFinishContext
    {
        public LevelOutcome Outcome { get; private set; }
        public int Stars { get; private set; }
        public long DurationMs { get; private set; }
        public int Coins { get; private set; }

        public event Action RetryRequested;

        public void SetResult(LevelOutcome outcome, int stars, long durationMs, int coins)
        {
            Outcome = outcome;
            Stars = stars;
            DurationMs = durationMs;
            Coins = coins;
        }

        public void RequestRetry()
        {
            RetryRequested?.Invoke();
        }
    }
}
