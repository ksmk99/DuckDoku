using System;

namespace DuckDoku.Domain
{
    public interface ILevelStartSignal
    {
        bool HasStarted { get; }

        event Action Started;

        void MarkStarted();
    }
}
