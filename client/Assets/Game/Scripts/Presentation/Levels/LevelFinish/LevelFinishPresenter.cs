using System;
using DuckDoku.Domain;
using Zenject;

namespace DuckDoku.Presentation
{
    public class LevelFinishPresenter : IInitializable, IDisposable
    {
        private readonly LevelFinishView _view;
        private readonly ILevelFinishContext _finishContext;
        private readonly ILevelLauncher _levelLauncher;

        public LevelFinishPresenter(
            LevelFinishView view,
            ILevelFinishContext finishContext,
            ILevelLauncher levelLauncher)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (finishContext == null)
            {
                throw new ArgumentNullException(nameof(finishContext));
            }

            if (levelLauncher == null)
            {
                throw new ArgumentNullException(nameof(levelLauncher));
            }

            _view = view;
            _finishContext = finishContext;
            _levelLauncher = levelLauncher;
        }

        public void Initialize()
        {
            _view.ContinueRequested += OnContinueRequested;
            _view.NextRequested += OnNextRequested;

            if (_finishContext.Outcome == LevelOutcome.Victory)
            {
                _view.ShowVictory(_finishContext.Stars, _finishContext.Coins);
            }
            else
            {
                _view.ShowDefeat();
            }
        }

        public void Dispose()
        {
            _view.ContinueRequested -= OnContinueRequested;
            _view.NextRequested -= OnNextRequested;
        }

        private void OnContinueRequested()
        {
            _levelLauncher.ReturnToMap();
        }

        private void OnNextRequested()
        {
            _levelLauncher.ReturnToMap();
        }
    }
}
