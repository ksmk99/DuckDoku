using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class HintWalletService : IHintWalletService, IRemoteLoadable
    {
        private readonly IHintWalletClient _hintWalletClient;

        private int _count;

        public event Action Changed;
        public event Action<int> Denied;

        public int Count => _count;

        public HintWalletService(IHintWalletClient hintWalletClient)
        {
            if (hintWalletClient == null)
            {
                throw new ArgumentNullException(nameof(hintWalletClient));
            }

            _hintWalletClient = hintWalletClient;
        }

        public void Apply(int count)
        {
            _count = count;

            Changed?.Invoke();
        }

        public bool HasEnough(int cost)
        {
            return _count >= cost;
        }

        public void NotifyDenied(int cost)
        {
            Denied?.Invoke(cost);
        }

        public async UniTask LoadAsync(CancellationToken cancellationToken)
        {
            HintsStateResponse response = await _hintWalletClient.GetState(cancellationToken);
            Apply(response.hints);
        }
    }
}
