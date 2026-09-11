using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class EnergyPresenter : IInitializable, IDisposable
    {
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

        private readonly EnergyView _view;
        private readonly IEnergyService _energyService;
        private readonly IPopupService _popupService;
        private readonly EnergyPopupView.Factory _popupFactory;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public EnergyPresenter(EnergyView view,
            IEnergyService energyService,
            IPopupService popupService,
            EnergyPopupView.Factory popupFactory)
        {
            _view = view;
            _energyService = energyService;
            _popupService = popupService;
            _popupFactory = popupFactory;
        }

        public void Initialize()
        {
            _energyService.Changed += Render;
            _energyService.Denied += OnDenied;

            Render();

            TickAsync(_cts.Token).Forget();
        }

        public void Dispose()
        {
            _energyService.Changed -= Render;
            _energyService.Denied -= OnDenied;

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

        private void OnDenied(int cost)
        {
            _view.Pulse();
            ShowPopupAsync().Forget();
        }

        private async UniTaskVoid ShowPopupAsync()
        {
            EnergyPopupView popup = _popupFactory.Create();
            EnergySnapshot snapshot = _energyService.Current;
            
            popup.SetCountdown(_energyService.TimeUntilNext);

            await _popupService.Show(popup);
        }
    }
}
