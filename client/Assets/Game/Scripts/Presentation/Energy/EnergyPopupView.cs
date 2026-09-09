using System;
using TMPro;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class EnergyPopupView : PopupView
    {
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private TMP_Text _countdownText;

        public void SetValue(int value, int max)
        {
            _valueText.text = $"{value}/{max}";
        }

        public void SetCountdown(TimeSpan? remaining)
        {
            if (remaining is null)
            {
                _countdownText.gameObject.SetActive(false);
                return;
            }

            _countdownText.gameObject.SetActive(true);
            _countdownText.text = remaining.Value.ToString(@"mm\:ss");
        }
    }
}
