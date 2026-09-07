using System;

namespace DuckDoku.Domain
{
    public interface ILevelFinishContext
    {
        LevelOutcome Outcome { get; }
        int Stars { get; }
        long DurationMs { get; }

        event Action RetryRequested;

        void SetResult(LevelOutcome outcome, int stars, long durationMs);
        void RequestRetry();
    }
}
