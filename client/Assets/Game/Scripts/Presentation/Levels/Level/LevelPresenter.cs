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
        private readonly IEnergyService _energyService;

        public LevelPresenter(LevelView levelView, LevelModel model, ILevelLauncher levelLauncher,
            IEnergyService energyService)
        {
            _levelView = levelView;
            _model = model;
            _levelLauncher = levelLauncher;
            _energyService = energyService;

            _levelView.Onclick += LaunchLevel;
        }

        private void LaunchLevel()
        {
            if (_model.LevelSummary.Status != LevelStatus.Unlocked)
            {
                return;
            }

            if (!_energyService.HasEnough(EnergyPolicy.EntryCost))
            {
                _energyService.NotifyDenied();
                return;
            }

            _levelLauncher.LaunchLevel(_model.LevelSummary.LevelId);
        }

        public void Dispose()
        {
            _levelView.Onclick -= LaunchLevel;
            GameObject.Destroy(_levelView);
        }
    }
}