using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class CurrencyClient : ICurrencyClient
    {
        private const string BalancePath = "/api/v1/currency";

        private readonly ServerConfig _serverConfig;
        private readonly PlayerSession _session;

        public CurrencyClient(ServerConfig serverConfig, PlayerSession session)
        {
            _serverConfig = serverConfig;
            _session = session;
        }

        public async UniTask<CurrencyStateResponse> GetBalance(CancellationToken cancellationToken = default)
        {
            if (!_session.IsAuthenticated)
            {
                throw new Exception("Not authenticated: call guest login first.");
            }

            using (UnityWebRequest request = UnityWebRequest.Get(_serverConfig.BaseUrl + BalancePath))
            {
                request.timeout = _serverConfig.RequestTimeoutSeconds;
                request.SetRequestHeader("Authorization", "Ducky " + _session.Token);

                UnityWebRequest result = await request.SendWebRequest().WithCancellation(cancellationToken);

                return JsonUtility.FromJson<CurrencyStateResponse>(result.downloadHandler.text);
            }
        }
    }
}
