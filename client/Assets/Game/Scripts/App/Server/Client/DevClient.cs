#if UNITY_EDITOR
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class DevClient : IDevClient
    {
        private const string GrantMaxEnergyPath = "/api/v1/dev/energy/max";
        private const string GrantCurrencyPath = "/api/v1/dev/currency/grant";

        private readonly ServerConfig _serverConfig;
        private readonly PlayerSession _session;

        public DevClient(ServerConfig serverConfig, PlayerSession session)
        {
            _serverConfig = serverConfig;
            _session = session;
        }

        public async UniTask<DevEnergyResponse> GrantMaxEnergy(CancellationToken cancellationToken = default)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(_serverConfig.BaseUrl + GrantMaxEnergyPath, string.Empty, "application/json"))
            {
                string json = await SendAsync(request, cancellationToken);
                return JsonUtility.FromJson<DevEnergyResponse>(json);
            }
        }

        public async UniTask<DevCurrencyResponse> GrantCurrency(CancellationToken cancellationToken = default)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(_serverConfig.BaseUrl + GrantCurrencyPath, string.Empty, "application/json"))
            {
                string json = await SendAsync(request, cancellationToken);
                return JsonUtility.FromJson<DevCurrencyResponse>(json);
            }
        }

        private async UniTask<string> SendAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            if (!_session.IsAuthenticated)
            {
                throw new Exception("Not authenticated: call guest login first.");
            }

            request.SetRequestHeader("Authorization", "Ducky " + _session.Token);

            UnityWebRequest result = await request.SendWebRequest().WithCancellation(cancellationToken);

            if (result.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Request to {result.url} failed ({result.responseCode}): {result.error}");
            }

            return result.downloadHandler.text;
        }
    }
}
#endif
