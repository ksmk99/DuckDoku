using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class LevelsScrollController : IInitializable, IDisposable, IMetaFrame
    {
        private const int BufferRows = 1;

        private readonly LevelsScrollData _data;
        private readonly ILevelMapSource _levelMapSource;
        private readonly ILevelEntryGate _entryGate;
        private readonly LevelView.Factory _levelViewFactory;

        private LevelViewPool _pool;
        private IReadOnlyList<LevelSummary> _levels;
        private readonly Dictionary<int, ActiveSlot> _active = new Dictionary<int, ActiveSlot>();

        private Vector2 _cellSize;
        private int _columns;

        private struct ActiveSlot
        {
            public LevelView View;
            public LevelPresenter Presenter;
        }

        public LevelsScrollController(LevelsScrollData data,
            ILevelMapSource levelMapSource,
            ILevelEntryGate entryGate,
            LevelView.Factory levelViewFactory)
        {
            _data = data;
            _levelMapSource = levelMapSource;
            _entryGate = entryGate;
            _levelViewFactory = levelViewFactory;
        }

        public void Initialize()
        {
            _pool = new LevelViewPool(_levelViewFactory);
        }

        private async UniTask Build()
        {
            try
            {
                _levels = await _levelMapSource.GetLevelsAsync();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return;
            }

            _columns = _data.Columns;
            _cellSize = CalculateCellSize(_columns);

            _data.Content.anchorMin = new Vector2(0f, 1f);
            _data.Content.anchorMax = new Vector2(1f, 1f);

            int totalRows = Mathf.CeilToInt(_levels.Count / (float)_columns);
            float contentHeight = totalRows * _cellSize.y + Mathf.Max(0, totalRows - 1) * _data.SpacingY
                                   + _data.PaddingTop + _data.PaddingBottom;
            _data.Content.sizeDelta = new Vector2(_data.Content.sizeDelta.x, contentHeight);

            _data.ScrollRect.onValueChanged.AddListener(OnScroll);

            ScrollToUnlockedLevel();
            UpdateVisibleRange();
        }

        private void ScrollToUnlockedLevel()
        {
            int unlockedIndex = FindUnlockedIndex();
            if (unlockedIndex < 0)
            {
                return;
            }

            float viewportHeight = _data.Viewport.rect.height;
            float contentHeight = _data.Content.rect.height;
            float scrollableHeight = Mathf.Max(0f, contentHeight - viewportHeight);

            if (scrollableHeight <= 0f)
            {
                return;
            }

            int row = unlockedIndex / _columns;
            float rowStep = _cellSize.y + _data.SpacingY;
            float rowCenterY = _data.PaddingTop + row * rowStep + _cellSize.y * 0.5f;

            float desiredOffsetY = Mathf.Clamp(rowCenterY - viewportHeight * 0.5f, 0f, scrollableHeight);
            _data.ScrollRect.verticalNormalizedPosition = 1f - desiredOffsetY / scrollableHeight;
        }

        private int FindUnlockedIndex()
        {
            for (int i = 0; i < _levels.Count; i++)
            {
                if (_levels[i].Status == LevelStatus.Unlocked)
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnScroll(Vector2 _)
        {
            UpdateVisibleRange();
        }

        private void UpdateVisibleRange()
        {
            if (_levels == null || _levels.Count == 0)
            {
                return;
            }

            float rowStep = _cellSize.y + _data.SpacingY;
            float viewportHeight = _data.Viewport.rect.height;
            float contentHeight = _data.Content.rect.height;
            float scrollableHeight = Mathf.Max(0f, contentHeight - viewportHeight);
            float offsetY = Mathf.Clamp((1f - _data.ScrollRect.verticalNormalizedPosition) * scrollableHeight,
                0f, scrollableHeight);

            int totalRows = Mathf.CeilToInt(_levels.Count / (float)_columns);
            int maxRow = Mathf.Max(0, totalRows - 1);
            int firstRow = Mathf.Clamp(Mathf.FloorToInt(offsetY / rowStep) - BufferRows, 0, maxRow);
            int rowsOnScreen = Mathf.CeilToInt(viewportHeight / rowStep) + BufferRows * 2;
            int lastRow = Mathf.Min(maxRow, firstRow + rowsOnScreen);

            int firstIndex = firstRow * _columns;
            int lastIndex = Mathf.Min(_levels.Count - 1, (lastRow + 1) * _columns - 1);

            ReleaseOutOfRange(firstIndex, lastIndex);
            AcquireInRange(firstIndex, lastIndex);
        }

        private void ReleaseOutOfRange(int firstIndex, int lastIndex)
        {
            List<int> toRelease = null;

            foreach (KeyValuePair<int, ActiveSlot> pair in _active)
            {
                if (pair.Key < firstIndex || pair.Key > lastIndex)
                {
                    (toRelease ??= new List<int>()).Add(pair.Key);
                }
            }

            if (toRelease == null)
            {
                return;
            }

            foreach (int index in toRelease)
            {
                ActiveSlot slot = _active[index];
                slot.Presenter.Dispose();
                _pool.Release(slot.View);
                _active.Remove(index);
            }
        }

        private void AcquireInRange(int firstIndex, int lastIndex)
        {
            for (int index = firstIndex; index <= lastIndex; index++)
            {
                if (_active.ContainsKey(index))
                {
                    continue;
                }

                LevelView view = _pool.Get();
                PositionView(view, index);

                LevelSummary summary = _levels[index];
                view.Setup(summary.LevelId, summary.Status, summary.Stars);

                var model = new LevelModel(summary);
                var presenter = new LevelPresenter(view, model, _entryGate);

                _active[index] = new ActiveSlot { View = view, Presenter = presenter };
            }
        }

        private void PositionView(LevelView view, int index)
        {
            int row = index / _columns;
            int col = index % _columns;

            float x = _data.PaddingLeft + col * (_cellSize.x + _data.SpacingX);
            float y = -(_data.PaddingTop + row * (_cellSize.y + _data.SpacingY));

            var rect = (RectTransform)view.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = _cellSize;
            rect.anchoredPosition = new Vector2(x, y);
        }

        private Vector2 CalculateCellSize(int columns)
        {
            Rect area = _data.Viewport.rect;

            float horizontal = area.width
                               - _data.PaddingLeft - _data.PaddingRight
                               - _data.SpacingX * (columns - 1);

            float side = Mathf.Floor(horizontal / columns);

            return new Vector2(side, _data.CellHeight);
        }

        public void Dispose()
        {
            _data.ScrollRect.onValueChanged.RemoveListener(OnScroll);

            foreach (KeyValuePair<int, ActiveSlot> pair in _active)
            {
                pair.Value.Presenter.Dispose();
                _pool.Release(pair.Value.View);
            }

            _active.Clear();
        }

        public void OpenFrame()
        {
            Build().Forget();
        }

        public void CloseFrame()
        {
            Dispose();
        }
    }
}
