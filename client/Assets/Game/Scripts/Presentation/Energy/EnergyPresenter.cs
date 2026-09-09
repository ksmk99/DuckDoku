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
        private readonly EnergyPopupView _popupPrefab;
        private readonly PopupRoot _popupRoot;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public EnergyPresenter(EnergyView view,
            IEnergyService energyService,
            IPopupService popupService,
            EnergyPopupView popupPrefab,
            PopupRoot popupRoot)
        {
            _view = view;
            _energyService = energyService;
            _popupService = popupService;
            _popupPrefab = popupPrefab;
            _popupRoot = popupRoot;
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

        private void OnDenied()
        {
            _view.Pulse();
            ShowPopupAsync().Forget();
        }

        private async UniTaskVoid ShowPopupAsync()
        {
            EnergyPopupView popup = GameObject.Instantiate(_popupPrefab, _popupRoot.transform);
            EnergySnapshot snapshot = _energyService.Current;

            popup.SetValue(snapshot.Value, snapshot.Max);
            popup.SetCountdown(_energyService.TimeUntilNext);

            await _popupService.Show(popup);
        }
    }
}
