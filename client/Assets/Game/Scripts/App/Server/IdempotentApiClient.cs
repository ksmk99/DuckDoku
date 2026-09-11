using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class IdempotentApiClient : IIdempotentApiClient
    {
        private readonly IApiRequestExecutor _api;
        private readonly IPendingRequestTracker _tracker;

        public IdempotentApiClient(IApiRequestExecutor api, IPendingRequestTracker tracker)
        {
            _api = api;
            _tracker = tracker;
        }

        public async UniTask<TResponse> PostIdempotentAsync<TRequest, TResponse>(
            string path,
            string actionKey,
            Func<Guid, TRequest> buildRequest,
            CancellationToken cancellationToken = default)
            where TRequest : IIdempotentRequest
        {
            Guid requestId = _tracker.GetOrCreateGuid(actionKey);
            TRequest body = buildRequest(requestId);

            try
            {
                TResponse response = await _api.PostAsync<TResponse>(path, body, cancellationToken);

                _tracker.Resolve(actionKey);

                return response;
            }
            catch (ApiErrorException)
            {
                _tracker.Resolve(actionKey);

                throw;
            }
        }
    }
}