using System;
using DuckDoku.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class LevelView : MonoBehaviour
    { 
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Image _stateImage;
        [Space]
        [SerializeField] private Sprite _lockedSprite;
        [SerializeField] private Sprite _unlockedSprite;
        [SerializeField] private Sprite _completedSprite;

        public event Action Onclick;
        
        public void Setup(int levelId, LevelStatus status)
        {
            _levelText.text = levelId.ToString();
            switch (status)
            {
                case LevelStatus.Unlocked:
                    _stateImage.sprite = _unlockedSprite;
                    break;
                case LevelStatus.Completed:
                    _stateImage.sprite = _completedSprite;
                    break;
                case LevelStatus.Locked:
                    _stateImage.sprite = _lockedSprite;
                    break;
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Click);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Click);
        }

        private void Click()
        {
            Onclick?.Invoke();
        }
    }
}