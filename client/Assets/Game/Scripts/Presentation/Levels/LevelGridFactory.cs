using System;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DuckDoku.Presentation
{
    public class LevelGridFactory : IInitializable, IDisposable
    {
        private readonly LevelFactoryGridData _data;
        private readonly ILevelMapSource _levelMapSource;
        private readonly ILevelLauncher _levelLauncher;
        private readonly IEnergyService _energyService;

        private LevelPresenter[] _levels;

        public LevelGridFactory(LevelFactoryGridData data,
            ILevelMapSource  levelMapSource,
            ILevelLauncher levelLauncher,
            IEnergyService energyService)
        {
            _data = data;
            _levelMapSource = levelMapSource;
            _levelLauncher = levelLauncher;
            _energyService = energyService;
        }

        public void Initialize()
        {
            Build().Forget();
        }

        private async UniTask Build()
        {
            var levelsStates = await _levelMapSource.GetLevelsAsync();
            Clear();
            
            _levels = new LevelPresenter[levelsStates.Count];

            _data.Layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _data.Layout.constraintCount = _data.RowCount;
            _data.Layout.cellSize = CalculateCellSize(_data.RowCount);

            for (int i = 0; i < levelsStates.Count; i++)
            {
                    LevelView view = GameObject.Instantiate(_data.LevelPrefab, _data.Grid);

                    view.Setup(levelsStates[i].LevelId, levelsStates[i].Status);
                    var model = new LevelModel(levelsStates[i]);
                    var presenter = new LevelPresenter(view, model, _levelLauncher, _energyService);

                    _levels[i] = presenter;
            }
        }

        public void Dispose()
        {
            Clear();
        }

        private void Clear()
        {
            if (_levels == null)
            {
                return;
            }

            for (int i = 0; i < _levels.Length; i++)
            {
                LevelPresenter presenter = _levels[i];
                if (presenter == null)
                {
                    continue;
                }

                presenter.Dispose();
            }

            _levels = null;
        }

        private Vector2 CalculateCellSize(int size)
        {
            Rect area = _data.Grid.rect;

            float horizontal = area.width
                               - _data.Layout.padding.left - _data.Layout.padding.right
                               - _data.Layout.spacing.x * (size - 1);

            float vertical = area.height
                             - _data.Layout.padding.top - _data.Layout.padding.bottom
                             - _data.Layout.spacing.y * (size - 1);

            float side = Mathf.Floor(Mathf.Min(horizontal, vertical) / size);

            return new Vector2(side, side);
        }
    }
}