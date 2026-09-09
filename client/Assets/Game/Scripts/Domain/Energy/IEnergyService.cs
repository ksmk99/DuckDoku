using System;

namespace DuckDoku.Domain
{
    public interface IEnergyService
    {
        EnergySnapshot Current { get; }
        TimeSpan? TimeUntilNext { get; }

        event Action Changed;
        event Action<int> Denied;

        void Apply(int value, int max, long refillMs);

        bool HasEnough(int cost);

        void NotifyDenied(int cost);
    }
}
