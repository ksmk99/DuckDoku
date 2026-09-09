using UnityEngine;

namespace DuckDoku.Presentation
{
    public class LivesView : MonoBehaviour
    {
        [SerializeField] private HeartView[] _hearts;
        [SerializeField] private LivesFeedbackConfig _feedback;

        private void Awake()
        {
            foreach (HeartView heart in _hearts)
            {
                heart.Setup(_feedback);
            }
        }

        public void ShowFull()
        {
            foreach (HeartView heart in _hearts)
            {
                heart.ShowFull();
            }
        }

        public void PlayLost(int index)
        {
            _hearts[index].PlayLost();
        }
    }
}
