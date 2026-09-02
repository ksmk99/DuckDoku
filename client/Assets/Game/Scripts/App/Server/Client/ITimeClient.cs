using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    [Serializable]
    public sealed class ServerTimeResponse
    {
        public long unixTimeMs;
    }
    
    public interface ITimeClient
    {
        UniTask<ServerTimeResponse> GetServerTimeAsync(CancellationToken cancellationToken = default);
    }
}