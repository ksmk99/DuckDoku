using System;
using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform _grid;
        [SerializeField] private GridLayoutGroup _layout;
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private ColorPaletteConfig _palette;

        private CellView[] _cells;
        private int _size;

        public event Action<int, int, int> CellPressed;
        public event Action<int, int, int> CellEntered;
        public event Action<int> CellReleased;
        public event Action<int, int> CellHoverEntered;
        public event Action<int, int> CellHoverExited;

        public void Build(int size, int[] regions, CellFeedbackConfig feedback)
        {
            if (regions == null)
            {
                throw new ArgumentNullException(nameof(regions));
            }

            if (regions.Length != size * size)
            {
                throw new ArgumentException(
                    $"Region map must contain {size * size} elements, got {regions.Length}.",
                    nameof(regions));
            }

            Clear();

            _size = size;
            _cells = new CellView[size * size];

            _layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _layout.constraintCount = size;
            _layout.cellSize = CalculateCellSize(size);

            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    int index = row * size + column;

                    CellView cell = Instantiate(_cellPrefab, _grid);

                    cell.Setup(this, row, column, _palette.Colors[regions[index] % _palette.Colors.Count], feedback);

                    cell.SetBorders(
                        IsBorder(regions, size, row, column, -1, 0),
                        IsBorder(regions, size, row, column, 1, 0),
                        IsBorder(regions, size, row, column, 0, -1),
                        IsBorder(regions, size, row, column, 0, 1));
                    cell.Show(CellState.Empty, false);

                    _cells[index] = cell;
                }
            }

            PlayIntroReveal(size, feedback);
        }

        private void PlayIntroReveal(int size, CellFeedbackConfig feedback)
        {
            int centerRow = (size - 1) / 2;
            int centerColumn = (size - 1) / 2;

            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    float delay = feedback.IntroWaveDelayStep * ChebyshevDistance(row, column, centerRow, centerColumn);
                    _cells[row * size + column].PlayIntroReveal(delay);
                }
            }
        }

        public void Show(int row, int column, CellState state, bool hasConflict)
        {
            _cells[row * _size + column].Show(state, hasConflict);
        }

        public void PlayCrossPainted(int row, int column)
        {
            _cells[row * _size + column].PlayCrossPainted();
        }

        public void PlayCorrectPlacement(int row, int column)
        {
            _cells[row * _size + column].PlayCorrectPlacement();
        }

        public void PlayWrongPlacement(int row, int column)
        {
            _cells[row * _size + column].PlayWrongPlacement();
        }

        public void PlayBlocked(int row, int column)
        {
            _cells[row * _size + column].PlayBlocked();
        }

        public void PlayHoverEnter(int row, int column)
        {
            _cells[row * _size + column].PlayHoverEnter();
        }

        public void PlayHoverExit(int row, int column)
        {
            _cells[row * _size + column].PlayHoverExit();
        }

        public void PlayGroupWave(int row, int column, float delay)
        {
            _cells[row * _size + column].PlayGroupWave(delay);
        }

        public void PlayHintAccent(int row, int column)
        {
            _cells[row * _size + column].PlayHintAccent();
        }

        public void PlayHintAccentClear(int row, int column)
        {
            _cells[row * _size + column].PlayHintAccentClear();
        }

        public void HandleCellPressed(int pointerId, int row, int column)
        {
            CellPressed?.Invoke(pointerId, row, column);
        }

        public void HandleCellEntered(int pointerId, int row, int column)
        {
            CellEntered?.Invoke(pointerId, row, column);
        }

        public void HandleCellReleased(int pointerId)
        {
            CellReleased?.Invoke(pointerId);
        }

        public void HandleCellHoverEnter(int row, int column)
        {
            CellHoverEntered?.Invoke(row, column);
        }

        public void HandleCellHoverExit(int row, int column)
        {
            CellHoverExited?.Invoke(row, column);
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (_cells == null)
            {
                return;
            }

            for (int i = 0; i < _cells.Length; i++)
            {
                CellView cell = _cells[i];
                if (cell == null)
                {
                    continue;
                }

                cell.Release();
                Destroy(cell.gameObject);
            }

            _cells = null;
        }

        private Vector2 CalculateCellSize(int size)
        {
            Rect area = _grid.rect;

            float horizontal = area.width
                               - _layout.padding.left - _layout.padding.right
                               - _layout.spacing.x * (size - 1);

            float vertical = area.height
                             - _layout.padding.top - _layout.padding.bottom
                             - _layout.spacing.y * (size - 1);

            float side = Mathf.Floor(Mathf.Min(horizontal, vertical) / size);

            return new Vector2(side, side);
        }

        private static int ChebyshevDistance(int rowA, int columnA, int rowB, int columnB)
        {
            return Mathf.Max(Mathf.Abs(rowA - rowB), Mathf.Abs(columnA - columnB));
        }

        private static bool IsBorder(int[] regions, int size, int row, int column, int rowStep, int columnStep)
        {
            int neighbourRow = row + rowStep;
            int neighbourColumn = column + columnStep;

            if (neighbourRow < 0 || neighbourRow >= size || neighbourColumn < 0 || neighbourColumn >= size)
            {
                return true;
            }

            return regions[neighbourRow * size + neighbourColumn] != regions[row * size + column];
        }
    }
}
