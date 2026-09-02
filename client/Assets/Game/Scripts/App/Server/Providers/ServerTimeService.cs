using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DuckDoku.App
{
    public sealed class ServerTimeService : IServerTimeService
    {
        private readonly ITimeClient _timeClient;

        private double _serverTimeAtSync;
        private double _localTimeAtSync;

        public bool IsSynchronized { get; private set; }

        public DateTimeOffset UtcNow
        {
            get
            {
                if (!IsSynchronized)
                {
                    throw new InvalidOperationException(
                        $"{nameof(ServerTimeService)} is not synchronized yet — call {nameof(SynchronizeAsync)} first.");
                }
                
                var elapsed = Time.realtimeSinceStartupAsDouble - _localTimeAtSync;
                var unixMilliseconds = _serverTimeAtSync + elapsed * 1000d;

                return DateTimeOffset.FromUnixTimeMilliseconds(
                    (long)unixMilliseconds);
            }
        }

        public long UnixTimeMilliseconds =>
            UtcNow.ToUnixTimeMilliseconds();

        public ServerTimeService(ITimeClient timeClient)
        {
            _timeClient = timeClient;
        }

        public async UniTask SynchronizeAsync(
            CancellationToken cancellationToken = default)
        {
            var beforeRequest = Time.realtimeSinceStartupAsDouble;

            var response = await _timeClient.GetServerTimeAsync(cancellationToken);

            var afterRequest = Time.realtimeSinceStartupAsDouble;
            var requestDuration = afterRequest - beforeRequest;
            var networkDelay = requestDuration / 2d;

            _serverTimeAtSync = response.unixTimeMs + networkDelay * 1000d;
            _localTimeAtSync = afterRequest;

            IsSynchronized = true;
        }
    }
}