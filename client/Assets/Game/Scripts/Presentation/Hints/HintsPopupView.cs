using TMPro;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class HintsPopupView : PopupView
    {
        [SerializeField] private TMP_Text _priceText;

        public void SetPrice(int cost)
        {
            _priceText.text = $"A hint costs {cost} coins.";
        }

        public class Factory : PlaceholderFactory<HintsPopupView>
        {
        }
    }
}
