using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class EnergyClient : IEnergyClient
    {
        private const string StatePath = "/api/v1/energy";

        private readonly IApiRequestExecutor _api;

        public EnergyClient(IApiRequestExecutor api)
        {
            _api = api;
        }

        public UniTask<EnergyStateResponse> GetState(CancellationToken cancellationToken = default)
        {
            return _api.GetAsync<EnergyStateResponse>(StatePath, cancellationToken);
        }
    }
}
