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
        [SerializeField] private Color[] _palette;

        private CellView[] _cells;
        private int _size;

        public event Action<int, int, int> CellPressed;
        public event Action<int, int, int> CellEntered;
        public event Action<int> CellReleased;

        public void Build(int size, int[] regions)
        {
            if (regions == null)
            {
                throw new ArgumentNullException(nameof(regions));
            }

            if (regions.Length != size * size)
            {
                throw new ArgumentException(
                    $"Карта областей должна содержать {size * size} элементов, получено {regions.Length}.",
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

                    cell.Setup(this, row, column, _palette[regions[index] % _palette.Length]);

                    cell.SetBorders(
                        IsBorder(regions, size, row, column, -1, 0),
                        IsBorder(regions, size, row, column, 1, 0),
                        IsBorder(regions, size, row, column, 0, -1),
                        IsBorder(regions, size, row, column, 0, 1));
                    cell.Show(CellState.Empty, false);

                    _cells[index] = cell;
                }
            }
        }

        public void Show(int row, int column, CellState state, bool hasConflict)
        {
            _cells[row * _size + column].Show(state, hasConflict);
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
