using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class CurrencyClient : ICurrencyClient
    {
        private const string BalancePath = "/api/v1/currency";

        private readonly IApiRequestExecutor _api;

        public CurrencyClient(IApiRequestExecutor api)
        {
            _api = api;
        }

        public UniTask<CurrencyStateResponse> GetBalance(CancellationToken cancellationToken = default)
        {
            return _api.GetAsync<CurrencyStateResponse>(BalancePath, cancellationToken);
        }
    }
}
