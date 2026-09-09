using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class EnergyService : IEnergyService, IRemoteLoadable
    {
        private readonly IServerTimeService _serverTimeService;
        private readonly IEnergyClient _energyClient;

        private EnergySnapshot _lastKnown;

        public event Action Changed;
        public event Action<int> Denied;

        public EnergySnapshot Current => EnergyPolicy.Project(_lastKnown, _serverTimeService.UtcNow);

        public TimeSpan? TimeUntilNext => Current.RefillAt is null
            ? null
            : Current.RefillAt.Value - _serverTimeService.UtcNow;

        public EnergyService(IServerTimeService serverTimeService, IEnergyClient energyClient)
        {
            if (energyClient == null)
            {
                throw new ArgumentNullException(nameof(energyClient));
            }

            _serverTimeService = serverTimeService;
            _energyClient = energyClient;
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

        public void NotifyDenied(int cost)
        {
            Denied?.Invoke(cost);
        }

        public async UniTask LoadAsync(CancellationToken cancellationToken)
        {
            EnergyStateResponse response = await _energyClient.GetState(cancellationToken);
            Apply(response.energy, response.energyMax, response.energyRefillMs);
        }
    }
}
