using Cysharp.Threading.Tasks;

namespace DuckDoku.Presentation
{
    public class ClickCooldown
    {
        private readonly int _durationMs;
        private bool _isActive;

        public ClickCooldown(int durationMs)
        {
            _durationMs = durationMs;
        }

        public bool TryConsume()
        {
            if (_isActive)
            {
                return false;
            }

            _isActive = true;
            ResetAfterDelayAsync().Forget();

            return true;
        }

        private async UniTaskVoid ResetAfterDelayAsync()
        {
            await UniTask.Delay(_durationMs);

            _isActive = false;
        }
    }
}
