using System;
using DuckDoku.Domain;
using Zenject;

namespace DuckDoku.Presentation
{
    public class LevelIndicatorPresenter : IInitializable
    {
        private readonly LevelIndicatorView _view;
        private readonly ILevelLauncher _levelLauncher;

        public LevelIndicatorPresenter(LevelIndicatorView view, ILevelLauncher levelLauncher)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (levelLauncher == null)
            {
                throw new ArgumentNullException(nameof(levelLauncher));
            }

            _view = view;
            _levelLauncher = levelLauncher;
        }

        public void Initialize()
        {
            _view.SetLevel(_levelLauncher.GetLevelId());
        }
    }
}
