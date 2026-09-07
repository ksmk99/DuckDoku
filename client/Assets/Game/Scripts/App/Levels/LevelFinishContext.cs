using System;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class LevelFinishContext : ILevelFinishContext
    {
        public LevelOutcome Outcome { get; private set; }
        public int Stars { get; private set; }
        public long DurationMs { get; private set; }

        public event Action RetryRequested;

        public void SetResult(LevelOutcome outcome, int stars, long durationMs)
        {
            Outcome = outcome;
            Stars = stars;
            DurationMs = durationMs;
        }

        public void RequestRetry()
        {
            RetryRequested?.Invoke();
        }
    }
}
