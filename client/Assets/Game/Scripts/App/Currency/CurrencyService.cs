using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class CurrencyService : ICurrencyService, IRemoteLoadable
    {
        private readonly ICurrencyClient _currencyClient;

        private int _balance;

        public event Action Changed;
        public event Action<int> Denied;

        public int Balance => _balance;

        public CurrencyService(ICurrencyClient currencyClient)
        {
            if (currencyClient == null)
            {
                throw new ArgumentNullException(nameof(currencyClient));
            }

            _currencyClient = currencyClient;
        }

        public void Apply(int balance)
        {
            _balance = balance;

            Changed?.Invoke();
        }

        public bool HasEnough(int cost)
        {
            return _balance >= cost;
        }

        public void NotifyDenied(int cost)
        {
            Denied?.Invoke(cost);
        }

        public async UniTask LoadAsync(CancellationToken cancellationToken)
        {
            CurrencyStateResponse response = await _currencyClient.GetBalance(cancellationToken);
            Apply(response.balance);
        }
    }
}
