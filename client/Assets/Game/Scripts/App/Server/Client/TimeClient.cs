using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class TimeClient : ITimeClient
    {
        private const string TimePath = "/api/v1/time";

        private readonly IApiRequestExecutor _api;

        public TimeClient(IApiRequestExecutor api)
        {
            _api = api;
        }

        public async UniTask<ServerTimeResponse> GetServerTimeAsync(CancellationToken cancellationToken = default)
        {
            ServerTimeResponse response = await _api.GetAsync<ServerTimeResponse>(TimePath, cancellationToken);

            if (response == null)
            {
                throw new Exception("Server time response is empty.");
            }

            return response;
        }
    }
}
