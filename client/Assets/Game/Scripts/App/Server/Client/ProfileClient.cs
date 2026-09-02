using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class ProfileClient : IProfileClient
    {
        private const string ProfilePath = "/api/v1/profile";
        private const string ChangeNamePath = "/api/v1/profile/name";

        private readonly ServerConfig _serverConfig;
        private readonly PlayerSession _session;

        public ProfileClient(ServerConfig serverConfig,
            PlayerSession session)
        {
            _serverConfig = serverConfig;
            _session = session;
        }

        public async UniTask<PlayerProfile> GetProfile(CancellationToken cancellationToken = default)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(_serverConfig.BaseUrl + ProfilePath))
            {
                return await SendAsync(request, cancellationToken);
            }
        }

        public async UniTask<PlayerProfile> ChangeName(string name, CancellationToken cancellationToken = default)
        {
            string body = JsonUtility.ToJson(new ChangeNameRequest() { displayName = name });
            using (UnityWebRequest request = UnityWebRequest.Put(_serverConfig.BaseUrl + ChangeNamePath, body))
            {
                request.SetRequestHeader("Content-Type", "application/json");

                return await SendAsync(request, cancellationToken);
            }
        }

        private async UniTask<PlayerProfile> SendAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            if (!_session.IsAuthenticated)
            {
                throw new Exception("Not authenticated: call guest login first.");
            }

            request.SetRequestHeader("Authorization", "Ducky " + _session.Token);

            UnityWebRequest result = await request.SendWebRequest().WithCancellation(cancellationToken);

            PlayerProfile profile = JsonUtility.FromJson<PlayerProfile>(result.downloadHandler.text);
            if (profile == null || string.IsNullOrEmpty(profile.playerId))
            {
                throw new Exception("Profile response is empty.");
            }

            return profile;
        }

        [Serializable]
        private class ChangeNameRequest
        {
            public string displayName;
        }
    }
}
