using UnityEngine;

namespace DuckDoku.Presentation
{
    public class RulesStripView : MonoBehaviour
    {
        [SerializeField] private RuleCardView _cardPrefab;
        [SerializeField] private RuleCardConfig[] _cards;

        private void Awake()
        {
            for (int i = 0; i < _cards.Length; i++)
            {
                RuleCardView card = Instantiate(_cardPrefab, transform);
                card.Build(_cards[i]);
            }
        }
    }
}
