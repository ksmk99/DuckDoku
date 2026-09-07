using System;

namespace DuckDoku.Domain
{
    public class MistakeTracker
    {
        public const int MaxMistakes = 3;

        public event Action<int> MistakeMade;
        public event Action Failed;

        public int MistakeCount { get; private set; }

        public bool IsFailed => MistakeCount >= MaxMistakes;

        public void RegisterMistake()
        {
            if (IsFailed)
            {
                return;
            }

            MistakeCount++;

            MistakeMade?.Invoke(MaxMistakes - MistakeCount);

            if (IsFailed)
            {
                Failed?.Invoke();
            }
        }
    }
}
