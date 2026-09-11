using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class EnergyPopupView : PopupView
    {
        [SerializeField] private TMP_Text _countdownText;

        public void SetCountdown(TimeSpan? remaining)
        {
            if (remaining is null)
            {
                _countdownText.gameObject.SetActive(false);
                return;
            }

            _countdownText.gameObject.SetActive(true);
            _countdownText.text = $"Refills in {remaining.Value.ToString(@"mm\:ss")}";
        }

        public class Factory : PlaceholderFactory<EnergyPopupView>
        {
        }
    }
}
