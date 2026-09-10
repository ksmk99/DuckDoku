using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class RuleCardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private RectTransform _grid;
        [SerializeField] private GridLayoutGroup _layout;
        [SerializeField] private RuleCellView _cellPrefab;

        private RuleCellView[] _cells;

        public void Build(RuleCardConfig config)
        {
            Clear();

            _titleText.text = config.Title;

            int size = config.Size;
            IReadOnlyList<RuleCellConfig> cells = config.Cells;
            IReadOnlyList<Color> regionColors = config.RegionColors;

            _layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _layout.constraintCount = size;
            _layout.cellSize = CalculateCellSize(size);

            _cells = new RuleCellView[cells.Count];

            for (int i = 0; i < cells.Count; i++)
            {
                RuleCellConfig cell = cells[i];

                Color regionColor = regionColors.Count > 0
                    ? regionColors[cell.Region % regionColors.Count]
                    : Color.white;

                RuleCellView view = Instantiate(_cellPrefab, _grid);
                view.SetColor(regionColor);
                view.Show(cell.State);

                _cells[i] = view;
            }
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
                RuleCellView cell = _cells[i];
                if (cell == null)
                {
                    continue;
                }

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
    }
}
