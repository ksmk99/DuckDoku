using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace DuckDoku.Presentation
{
    public class ButtonPressFeedback : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private ButtonFeedbackConfig _feedback;

        private ISfxPlayer _sfxPlayer;

        private bool _isPressed;
        private bool _isHovering;

        [Inject]
        private void Construct(ISfxPlayer sfxPlayer)
        {
            _sfxPlayer = sfxPlayer;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.interactable || _isPressed)
            {
                return;
            }

            _isHovering = true;

            transform.DOKill();
            transform.DOScale(_feedback.HoverScale, _feedback.HoverDuration).SetEase(_feedback.HoverEase);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            _isPressed = false;

            transform.DOKill();
            transform.DOScale(1f, _feedback.ReleaseDuration).SetEase(_feedback.ReleaseEase);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.interactable)
            {
                return;
            }

            _isPressed = true;

            _sfxPlayer.Play(SfxId.ButtonClick);

            transform.DOKill();
            transform.DOScale(_feedback.PressScale, _feedback.PressDuration).SetEase(_feedback.PressEase);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isPressed)
            {
                return;
            }

            _isPressed = false;

            transform.DOKill();

            float targetScale = _isHovering ? _feedback.HoverScale : 1f;
            transform.DOScale(targetScale, _feedback.ReleaseDuration).SetEase(_feedback.ReleaseEase);
        }

        private void OnDisable()
        {
            _isPressed = false;
            _isHovering = false;

            transform.DOKill();
            transform.localScale = Vector3.one;
        }
    }
}
