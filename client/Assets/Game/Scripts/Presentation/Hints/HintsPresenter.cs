using System;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class HintsPresenter : IInitializable, IDisposable
    {
        private readonly HintsView _view;
        private readonly IHintWalletService _hintWalletService;
        private readonly ICurrencyService _currencyService;
        private readonly IPopupService _popupService;
        private readonly HintsPopupView _popupPrefab;
        private readonly PopupRoot _popupRoot;

        public HintsPresenter(HintsView view,
            IHintWalletService hintWalletService,
            ICurrencyService currencyService,
            IPopupService popupService,
            HintsPopupView popupPrefab,
            PopupRoot popupRoot)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (hintWalletService == null)
            {
                throw new ArgumentNullException(nameof(hintWalletService));
            }

            if (currencyService == null)
            {
                throw new ArgumentNullException(nameof(currencyService));
            }

            if (popupService == null)
            {
                throw new ArgumentNullException(nameof(popupService));
            }

            if (popupPrefab == null)
            {
                throw new ArgumentNullException(nameof(popupPrefab));
            }

            if (popupRoot == null)
            {
                throw new ArgumentNullException(nameof(popupRoot));
            }

            _view = view;
            _hintWalletService = hintWalletService;
            _currencyService = currencyService;
            _popupService = popupService;
            _popupPrefab = popupPrefab;
            _popupRoot = popupRoot;
        }

        public void Initialize()
        {
            _hintWalletService.Changed += Render;
            _currencyService.Denied += OnDenied;

            Render();
        }

        public void Dispose()
        {
            _hintWalletService.Changed -= Render;
            _currencyService.Denied -= OnDenied;
        }

        private void Render()
        {
            int count = _hintWalletService.Count;

            if (count > 0)
            {
                _view.ShowCount(count);
            }
            else
            {
                _view.ShowPrice(HintPolicy.Cost);
            }
        }

        private void OnDenied(int cost)
        {
            _view.Pulse();
            ShowPopupAsync(cost).Forget();
        }

        private async UniTaskVoid ShowPopupAsync(int cost)
        {
            HintsPopupView popup = GameObject.Instantiate(_popupPrefab, _popupRoot.transform);
            popup.SetPrice(cost);

            await _popupService.Show(popup);
        }
    }
}
