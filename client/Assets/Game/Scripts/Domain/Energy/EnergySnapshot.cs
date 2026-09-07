using System;

namespace DuckDoku.Domain
{
    public readonly struct EnergySnapshot
    {
        public int Value { get; }
        public int Max { get; }
        public DateTimeOffset? RefillAt { get; }

        public EnergySnapshot(int value, int max, DateTimeOffset? refillAt)
        {
            Value = value;
            Max = max;
            RefillAt = refillAt;
        }
    }
}
