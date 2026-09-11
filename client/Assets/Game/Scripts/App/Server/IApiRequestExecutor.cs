using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IApiRequestExecutor
    {
        UniTask<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken,
            bool requireAuth = true);

        UniTask<TResponse> PostAsync<TResponse>(string path, object body, CancellationToken cancellationToken,
            bool requireAuth = true);

        UniTask<TResponse> PutAsync<TResponse>(string path, object body, CancellationToken cancellationToken,
            bool requireAuth = true);
    }
}
