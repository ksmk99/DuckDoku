using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class TimeClient : ITimeClient
    {
        private const string TimePath = "/api/v1/time";

        private readonly ServerConfig _serverConfig;
        private readonly PlayerSession _session;

        public TimeClient(ServerConfig serverConfig, PlayerSession session)
        {
            _serverConfig = serverConfig;
            _session = session;
        }

        public async UniTask<ServerTimeResponse> GetServerTimeAsync(CancellationToken cancellationToken = default)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(_serverConfig.BaseUrl + TimePath))
            {
                return await SendAsync(request, cancellationToken);
            }
        }

        private async UniTask<ServerTimeResponse> SendAsync(UnityWebRequest request,
            CancellationToken cancellationToken)
        {
            if (!_session.IsAuthenticated)
            {
                throw new Exception("Not authenticated: call guest login first.");
            }

            request.timeout = _serverConfig.RequestTimeoutSeconds;
            request.SetRequestHeader("Authorization", "Ducky " + _session.Token);

            UnityWebRequest result = await request.SendWebRequest().WithCancellation(cancellationToken);

            ServerTimeResponse timeResponse = JsonUtility.FromJson<ServerTimeResponse>(result.downloadHandler.text);
            if (timeResponse == null)
            {
                throw new Exception("Error getting server time");
            }

            return timeResponse;
        }
    }
}