using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class EnergyClient : IEnergyClient
    {
        private const string StatePath = "/api/v1/energy";

        private readonly ServerConfig _serverConfig;
        private readonly PlayerSession _session;

        public EnergyClient(ServerConfig serverConfig, PlayerSession session)
        {
            _serverConfig = serverConfig;
            _session = session;
        }

        public async UniTask<EnergyStateResponse> GetState(CancellationToken cancellationToken = default)
        {
            if (!_session.IsAuthenticated)
            {
                throw new Exception("Not authenticated: call guest login first.");
            }

            using (UnityWebRequest request = UnityWebRequest.Get(_serverConfig.BaseUrl + StatePath))
            {
                request.timeout = _serverConfig.RequestTimeoutSeconds;
                request.SetRequestHeader("Authorization", "Ducky " + _session.Token);

                UnityWebRequest result = await request.SendWebRequest().WithCancellation(cancellationToken);

                return JsonUtility.FromJson<EnergyStateResponse>(result.downloadHandler.text);
            }
        }
    }
}
