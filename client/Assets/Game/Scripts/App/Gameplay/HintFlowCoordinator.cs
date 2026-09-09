using System;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class HintFlowCoordinator : IInitializable, IDisposable
    {
        private readonly HintsView _hintsView;
        private readonly HintService _hintService;
        private readonly IHintWalletService _hintWalletService;
        private readonly ILevelSessionService _levelSessionService;

        private int _levelId;
        private string _sessionId;
        private bool _isHinting;

        public HintFlowCoordinator(HintsView hintsView,
            HintService hintService,
            IHintWalletService hintWalletService,
            ILevelSessionService levelSessionService)
        {
            if (hintsView == null)
            {
                throw new ArgumentNullException(nameof(hintsView));
            }

            if (hintService == null)
            {
                throw new ArgumentNullException(nameof(hintService));
            }

            if (hintWalletService == null)
            {
                throw new ArgumentNullException(nameof(hintWalletService));
            }

            if (levelSessionService == null)
            {
                throw new ArgumentNullException(nameof(levelSessionService));
            }

            _hintsView = hintsView;
            _hintService = hintService;
            _hintWalletService = hintWalletService;
            _levelSessionService = levelSessionService;
        }

        public void Initialize()
        {
            _hintsView.HintRequested += OnHintRequested;
            _hintService.Changed += OnHintServiceChanged;
        }

        public void Dispose()
        {
            _hintsView.HintRequested -= OnHintRequested;
            _hintService.Changed -= OnHintServiceChanged;
        }

        public void Attach(int levelId, string sessionId)
        {
            _levelId = levelId;
            _sessionId = sessionId;

            _hintsView.SetInteractable(true);
        }

        private void OnHintRequested()
        {
            if (_isHinting || _hintService.ActiveHint != null)
            {
                return;
            }

            _isHinting = true;
            HintAsync().Forget();
        }

        private void OnHintServiceChanged()
        {
            _hintsView.SetInteractable(_hintService.ActiveHint == null);
        }

        private async UniTaskVoid HintAsync()
        {
            _hintsView.SetInteractable(false);

            try
            {
                bool granted = _hintWalletService.Count > 0
                    ? await _levelSessionService.UseHintAsync(_levelId, _sessionId)
                    : await PurchaseAndUseHintAsync();

                if (granted)
                {
                    _hintService.TryRequestHint();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                _isHinting = false;
                _hintsView.SetInteractable(_hintService.ActiveHint == null);
            }
        }

        private async UniTask<bool> PurchaseAndUseHintAsync()
        {
            bool purchased = await _levelSessionService.PurchaseHintAsync();

            if (!purchased)
            {
                return false;
            }

            return await _levelSessionService.UseHintAsync(_levelId, _sessionId);
        }
    }
}
