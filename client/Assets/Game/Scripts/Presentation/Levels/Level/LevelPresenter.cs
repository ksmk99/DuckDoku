using System;
using DuckDoku.Domain;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class LevelPresenter : IDisposable
    {
        private readonly LevelView _levelView;
        private readonly LevelModel _model;
        private readonly ILevelLauncher _levelLauncher;

        public LevelPresenter(LevelView levelView, LevelModel model, ILevelLauncher levelLauncher)
        {
            _levelView = levelView;
            _model = model;
            _levelLauncher = levelLauncher;

            _levelView.Onclick += LaunchLevel;
        }

        private void LaunchLevel()
        {
            if (_model.LevelSummary.Status == LevelStatus.Unlocked)
            {
                _levelLauncher.LaunchLevel(_model.LevelSummary.LevelId);
            }
        }

        public void Dispose()
        {
            _levelView.Onclick -= LaunchLevel;
            GameObject.Destroy(_levelView);
        }
    }
}