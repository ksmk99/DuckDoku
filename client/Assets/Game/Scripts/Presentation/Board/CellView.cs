using DG.Tweening;
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
        private CellFeedbackConfig _feedback;
        private int _row;
        private int _column;
        private Color _regionColor;

        public void Setup(BoardView board, int row, int column, Color regionColor, CellFeedbackConfig feedback)
        {
            _board = board;
            _row = row;
            _column = column;
            _regionColor = regionColor;
            _feedback = feedback;

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

        public void PlayCrossPainted()
        {
            _content.transform.DOKill();

            _content.transform.localScale = Vector3.one * 0.7f;
            _content.transform.DOScale(Vector3.one, _feedback.CrossPulseDuration).SetEase(Ease.OutQuad);
            
            _background.transform.localScale = Vector3.one * 0.85f;
            _background.transform.DOScale(Vector3.one, _feedback.CrossPulseDuration).SetEase(Ease.OutQuad);
        }

        public void PlayCorrectPlacement()
        {
            _content.transform.DOKill();
            _background.DOKill();

            DOTween.Sequence()
                .Append(_content.transform.DOScale(Vector3.one * _feedback.AnticipationScale, _feedback.AnticipationDuration).SetEase(Ease.InQuad))
                .Append(_content.transform.DOScale(Vector3.one * _feedback.ImpactScale, _feedback.ImpactDuration).SetEase(Ease.OutBack))
                .Append(_content.transform.DOScale(Vector3.one, _feedback.SettleDuration).SetEase(Ease.OutQuad));

            _background.DOColor(_feedback.SuccessFlashColor, _feedback.ImpactDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _background.DOColor(_regionColor, _feedback.WaveSettleDuration).SetEase(Ease.InQuad));
        }

        public void PlayGroupWave(float delay)
        {
            _background.DOKill();

            _background.DOColor(_feedback.SuccessFlashColor, _feedback.WaveFlashDuration)
                .SetEase(Ease.OutQuad)
                .SetDelay(delay)
                .OnComplete(() => _background.DOColor(_regionColor, _feedback.WaveSettleDuration).SetEase(Ease.InQuad));
        }

        public void PlayWrongPlacement()
        {
            _content.transform.DOKill();
            _background.DOKill();

            _content.transform.DOShakePosition(_feedback.ShakeDuration, _feedback.ShakeStrength, _feedback.ShakeVibrato);
            _background.DOColor(Darken(_regionColor, _feedback.BlockedDarkenFactor), _feedback.BackgroundTintDuration).SetEase(Ease.OutQuad);
        }

        public void Release()
        {
            _content.transform.DOKill();
            _content.DOKill();
            _background.DOKill();

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

        private static Color Darken(Color color, float factor)
        {
            return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
        }
    }
}
