using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IIdempotentApiClient
    {
        UniTask<TResponse> PostIdempotentAsync<TRequest, TResponse>(
            string path,
            string actionKey,
            Func<Guid, TRequest> buildRequest,
            CancellationToken cancellationToken = default)
            where TRequest : IIdempotentRequest;
    }
}