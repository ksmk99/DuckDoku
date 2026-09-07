using System;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class EnergyService : IEnergyService
    {
        private readonly IServerTimeService _serverTimeService;

        private EnergySnapshot _lastKnown;

        public event Action Changed;
        public event Action Denied;

        public EnergySnapshot Current => EnergyPolicy.Project(_lastKnown, _serverTimeService.UtcNow);

        public TimeSpan? TimeUntilNext => Current.RefillAt is null
            ? null
            : Current.RefillAt.Value - _serverTimeService.UtcNow;

        public EnergyService(IServerTimeService serverTimeService)
        {
            _serverTimeService = serverTimeService;
            _lastKnown = new EnergySnapshot(0, 0, null);
        }

        public void Apply(int value, int max, long refillMs)
        {
            DateTimeOffset? refillAt = refillMs > 0
                ? _serverTimeService.UtcNow + TimeSpan.FromMilliseconds(refillMs)
                : null;

            _lastKnown = new EnergySnapshot(value, max, refillAt);

            Changed?.Invoke();
        }

        public bool HasEnough(int cost)
        {
            return Current.Value >= cost;
        }

        public void NotifyDenied()
        {
            Denied?.Invoke();
        }
    }
}
