using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class RuleCellView : ACellView
    {
        [SerializeField] private Image _duckImage;
        [SerializeField] private Image _crossImage;

        public override void Show(CellState state)
        {
            _duckImage.enabled = state == CellState.Duck;
            _crossImage.enabled = state == CellState.Cross;
        }
    }
}
