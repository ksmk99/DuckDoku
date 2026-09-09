using System;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using Zenject;

namespace DuckDoku.Presentation
{
    public class MainMenuPresenter : IInitializable, IDisposable
    {
        private readonly MainMenuView _view;
        private readonly ILevelMapSource _levelMapSource;
        private readonly ILevelEntryGate _entryGate;

        private bool _isStarting;

        public MainMenuPresenter(MainMenuView view, ILevelMapSource levelMapSource, ILevelEntryGate entryGate)
        {
            _view = view;
            _levelMapSource = levelMapSource;
            _entryGate = entryGate;
        }

        public void Initialize()
        {
            _view.PlayClicked += OnPlayClicked;
        }

        public void Dispose()
        {
            _view.PlayClicked -= OnPlayClicked;
        }

        private void OnPlayClicked()
        {
            if (_isStarting)
            {
                return;
            }

            _isStarting = true;
            PlayAsync().Forget();
        }

        private async UniTaskVoid PlayAsync()
        {
            try
            {
                int nextLevelId = await _levelMapSource.GetNextLevelIdAsync();

                if (nextLevelId == LevelMapPolicy.NoNextLevel)
                {
                    _view.PlayDenied();
                    return;
                }

                if (!_entryGate.TryStart(nextLevelId))
                {
                    _view.PlayDenied();
                }
            }
            finally
            {
                _isStarting = false;
            }
        }
    }
}
