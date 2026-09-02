using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class GuestAuthClient : IGuestAuthClient
    {
        private const string GuestAuthPath = "/api/v1/auth/guest";

        private readonly ServerConfig _serverConfig;
        private readonly IDeviceIdProvider _deviceIdProvider;
        private readonly PlayerSession _session;

        public GuestAuthClient(ServerConfig serverConfig,
            IDeviceIdProvider deviceIdProvider,
            PlayerSession session)
        {
            _serverConfig = serverConfig;
            _deviceIdProvider = deviceIdProvider;
            _session = session;
        }

        public async UniTask AuthenticateAsGuestAsync(CancellationToken cancellationToken = default)
        {
            GuestRequest payload = new GuestRequest()
            {
                deviceId = _deviceIdProvider.GetDeviceID()
            };

            string body = JsonUtility.ToJson(payload);
            string url = _serverConfig.BaseUrl + GuestAuthPath;

            using (UnityWebRequest request = UnityWebRequest.Post(url, body, "application/json"))
            {
                UnityWebRequest result = await request.SendWebRequest().WithCancellation(cancellationToken);

                GuestResponse response = JsonUtility.FromJson<GuestResponse>(result.downloadHandler.text);

                if (response == null || string.IsNullOrEmpty(response.playerId))
                {
                    throw new Exception($"Guest Login returned wrong body: {result.downloadHandler.text}");
                }

                _session.SetAuthentication(response.playerId, response.token);
            }
        }

        [Serializable]
        private class GuestRequest
        {
            public string deviceId;
        }

        [Serializable]
        private class GuestResponse
        {
            public string playerId;
            public string token;
        }
    }
}
