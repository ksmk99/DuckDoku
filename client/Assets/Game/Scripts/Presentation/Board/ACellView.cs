using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public abstract class ACellView : MonoBehaviour
    {
        [SerializeField] private Image _background;

        protected Image Background => _background;

        public virtual void SetColor(Color color)
        {
            _background.color = color;
        }

        public abstract void Show(CellState state);
    }
}
