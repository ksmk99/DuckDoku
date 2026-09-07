using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class CellView : MonoBehaviour,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerUpHandler
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _content;
        [SerializeField] private Image _conflictFrame;

        [SerializeField] private Sprite _crossSprite;
        [SerializeField] private Sprite _duckSprite;

        [SerializeField] private GameObject _borderTop;
        [SerializeField] private GameObject _borderBottom;
        [SerializeField] private GameObject _borderLeft;
        [SerializeField] private GameObject _borderRight;

        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _conflictColor = Color.red;
        [SerializeField] private Color _markColor = Color.white;
        [SerializeField] private Color _blockedColor = new Color(0.35f, 0.35f, 0.35f, 1f);

        private BoardView _board;
        private int _row;
        private int _column;

        public void Setup(BoardView board, int row, int column, Color regionColor)
        {
            _board = board;
            _row = row;
            _column = column;

            _background.color = regionColor;
        }

        public void SetBorders(bool top, bool bottom, bool left, bool right)
        {
            _borderTop.SetActive(top);
            _borderBottom.SetActive(bottom);
            _borderLeft.SetActive(left);
            _borderRight.SetActive(right);
        }

        public void Show(CellState state, bool hasConflict)
        {
            switch (state)
            {
                case CellState.Cross:
                    _content.enabled = true;
                    _content.sprite = _crossSprite;
                    _content.color = _markColor;
                    break;

                case CellState.Blocked:
                    _content.enabled = true;
                    _content.sprite = _crossSprite;
                    _content.color = _blockedColor;
                    break;

                case CellState.Duck:
                    _content.enabled = true;
                    _content.sprite = _duckSprite;
                    _content.color = Color.white;
                    break;

                default:
                    _content.enabled = false;
                    break;
            }

            _conflictFrame.color = hasConflict ? _conflictColor : _normalColor;
        }

        public void Release()
        {
            _board = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_board == null)
            {
                return;
            }

            _board.HandleCellPressed(eventData.pointerId, _row, _column);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_board == null || eventData.pointerPress == null)
            {
                return;
            }

            _board.HandleCellEntered(eventData.pointerId, _row, _column);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_board == null)
            {
                return;
            }

            _board.HandleCellReleased(eventData.pointerId);
        }
    }
}
