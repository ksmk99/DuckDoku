using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using Zenject;

namespace DuckDoku.Presentation
{
    public class EnergyPresenter : IInitializable, IDisposable
    {
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

        private readonly EnergyView _view;
        private readonly IEnergyService _energyService;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public EnergyPresenter(EnergyView view, IEnergyService energyService)
        {
            _view = view;
            _energyService = energyService;
        }

        public void Initialize()
        {
            _energyService.Changed += Render;
            _energyService.Denied += _view.Pulse;

            Render();

            TickAsync(_cts.Token).Forget();
        }

        public void Dispose()
        {
            _energyService.Changed -= Render;
            _energyService.Denied -= _view.Pulse;

            _cts.Cancel();
            _cts.Dispose();
        }

        private async UniTaskVoid TickAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TickInterval, cancellationToken: cancellationToken).SuppressCancellationThrow();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Render();
            }
        }

        private void Render()
        {
            EnergySnapshot snapshot = _energyService.Current;

            _view.SetValue(snapshot.Value, snapshot.Max);
            _view.SetCountdown(_energyService.TimeUntilNext);
        }
    }
}
