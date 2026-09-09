using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class HintWalletClient : IHintWalletClient
    {
        private const string StatePath = "/api/v1/hints";
        private const string PurchasePath = "/api/v1/hints/purchase";

        private readonly ServerConfig _serverConfig;
        private readonly PlayerSession _session;

        public HintWalletClient(ServerConfig serverConfig, PlayerSession session)
        {
            _serverConfig = serverConfig;
            _session = session;
        }

        public async UniTask<HintsStateResponse> GetState(CancellationToken cancellationToken = default)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(_serverConfig.BaseUrl + StatePath))
            {
                string json = await SendAsync(request, cancellationToken);
                return JsonUtility.FromJson<HintsStateResponse>(json);
            }
        }

        public async UniTask<PurchaseHintResponse> Purchase(CancellationToken cancellationToken = default)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(_serverConfig.BaseUrl + PurchasePath, string.Empty, "application/json"))
            {
                string json = await SendAsync(request, cancellationToken);
                return JsonUtility.FromJson<PurchaseHintResponse>(json);
            }
        }

        private async UniTask<string> SendAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            if (!_session.IsAuthenticated)
            {
                throw new Exception("Not authenticated: call guest login first.");
            }

            request.timeout = _serverConfig.RequestTimeoutSeconds;
            request.SetRequestHeader("Authorization", "Ducky " + _session.Token);

            UnityWebRequest result = await request.SendWebRequest().WithCancellation(cancellationToken);

            return result.downloadHandler.text;
        }
    }
}
