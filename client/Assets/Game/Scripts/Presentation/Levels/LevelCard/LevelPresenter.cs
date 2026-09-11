using System;
using DuckDoku.Domain;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class LevelPresenter : IDisposable
    {
        private const int ClickCooldownMs = 200;

        private readonly LevelView _levelView;
        private readonly LevelModel _model;
        private readonly ILevelEntryGate _entryGate;
        private readonly ClickCooldown _cooldown = new ClickCooldown(ClickCooldownMs);

        public LevelPresenter(LevelView levelView, LevelModel model, ILevelEntryGate entryGate)
        {
            _levelView = levelView;
            _model = model;
            _entryGate = entryGate;

            _levelView.Onclick += LaunchLevel;
        }

        private void LaunchLevel()
        {
            if (!_cooldown.TryConsume())
            {
                return;
            }

            if (_model.LevelSummary.Status == LevelStatus.Locked)
            {
                return;
            }

            _entryGate.TryStart(_model.LevelSummary.LevelId);
        }

        public void Dispose()
        {
            _levelView.Onclick -= LaunchLevel;
        }
    }
}