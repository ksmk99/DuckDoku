using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class HintsView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private GameObject _priceBadge;
        [SerializeField] private TMP_Text _priceValueText;
        [SerializeField] private Vector3 _punchStrength = new Vector3(0.2f, 0.2f, 0f);
        [SerializeField] private float _punchDuration = 0.25f;
        [SerializeField] private int _punchVibrato = 8;

        public event Action HintRequested;

        public void ShowCount(int count)
        {
            _valueText.gameObject.SetActive(true);
            _priceBadge.SetActive(false);
            _valueText.text = $"{count}";
        }

        public void ShowPrice(int cost)
        {
            _valueText.gameObject.SetActive(false);
            _priceBadge.SetActive(true);
            _priceValueText.text = $"{cost}";
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        public void Pulse()
        {
            transform.DOKill();
            transform.DOPunchScale(_punchStrength, _punchDuration, _punchVibrato);
        }

        private void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            HintRequested?.Invoke();
        }
    }
}
