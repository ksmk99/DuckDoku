using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IServerTimeService
    {
        DateTimeOffset UtcNow { get; }

        long UnixTimeMilliseconds { get; }

        bool IsSynchronized { get; }

        UniTask SynchronizeAsync(CancellationToken cancellationToken = default);
    }
}