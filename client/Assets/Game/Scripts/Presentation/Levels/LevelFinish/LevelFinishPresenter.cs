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
            _view.RetryRequested += OnRetryRequested;
            _view.NextRequested += OnNextRequested;

            if (_finishContext.Outcome == LevelOutcome.Victory)
            {
                _view.ShowVictory(_finishContext.Stars);
            }
            else
            {
                _view.ShowDefeat();
            }
        }

        public void Dispose()
        {
            _view.RetryRequested -= OnRetryRequested;
            _view.NextRequested -= OnNextRequested;
        }

        private void OnRetryRequested()
        {
            _finishContext.RequestRetry();
        }

        private void OnNextRequested()
        {
            _levelLauncher.ReturnToMap();
        }
    }
}
