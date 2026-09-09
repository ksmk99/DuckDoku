using System;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class LevelStartSignal : ILevelStartSignal
    {
        public bool HasStarted { get; private set; }

        public event Action Started;

        public void MarkStarted()
        {
            if (HasStarted)
            {
                return;
            }

            HasStarted = true;
            Started?.Invoke();
        }
    }
}
