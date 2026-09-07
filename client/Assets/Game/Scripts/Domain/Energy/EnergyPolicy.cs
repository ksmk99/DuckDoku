using System;

namespace DuckDoku.Domain
{
    public static class EnergyPolicy
    {
        public const int EntryCost = 1;

        private static readonly TimeSpan RecoveryPeriod = TimeSpan.FromMinutes(20);

        public static EnergySnapshot Project(EnergySnapshot snapshot, DateTimeOffset now)
        {
            if (snapshot.Value >= snapshot.Max || snapshot.RefillAt is null || now < snapshot.RefillAt.Value)
            {
                return snapshot;
            }

            TimeSpan overdue = now - snapshot.RefillAt.Value;
            int extraTicks = 1 + (int)(overdue / RecoveryPeriod);
            int value = Math.Min(snapshot.Max, snapshot.Value + extraTicks);
            DateTimeOffset? refillAt = value >= snapshot.Max ? null : snapshot.RefillAt.Value + RecoveryPeriod * extraTicks;

            return new EnergySnapshot(value, snapshot.Max, refillAt);
        }
    }
}
