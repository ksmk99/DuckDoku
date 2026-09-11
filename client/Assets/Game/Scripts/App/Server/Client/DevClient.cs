#if UNITY_EDITOR
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class DevClient : IDevClient
    {
        private const string GrantMaxEnergyPath = "/api/v1/dev/energy/max";
        private const string GrantCurrencyPath = "/api/v1/dev/currency/grant";

        private readonly IApiRequestExecutor _api;

        public DevClient(IApiRequestExecutor api)
        {
            _api = api;
        }

        public UniTask<DevEnergyResponse> GrantMaxEnergy(CancellationToken cancellationToken = default)
        {
            return _api.PostAsync<DevEnergyResponse>(GrantMaxEnergyPath, null, cancellationToken);
        }

        public UniTask<DevCurrencyResponse> GrantCurrency(CancellationToken cancellationToken = default)
        {
            return _api.PostAsync<DevCurrencyResponse>(GrantCurrencyPath, null, cancellationToken);
        }
    }
}
#endif
