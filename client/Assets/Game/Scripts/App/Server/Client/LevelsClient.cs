using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Puzzle;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class LevelsClient : ILevelsClient
    {
        private const string NextLevelPath = "/api/v1/levels/next";
        private const string StartLevelPath = "/api/v1/levels/{0}/start";
        private const string CompleteLevelPath = "/api/v1/levels/{0}/complete";
        private const string UseHintPath = "/api/v1/levels/{0}/hint";

        private readonly ServerConfig _serverConfig;
        private readonly PlayerSession _session;

        public LevelsClient(ServerConfig serverConfig, PlayerSession session)
        {
            _serverConfig = serverConfig;
            _session = session;
        }

        public async UniTask<NextLevelResponse> GetNextLevel(CancellationToken cancellationToken = default)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(_serverConfig.BaseUrl + NextLevelPath))
            {
                string json = await SendAsync(request, cancellationToken);
                return JsonUtility.FromJson<NextLevelResponse>(json);
            }
        }

        public async UniTask<StartLevelResponse> StartLevel(int levelId, CancellationToken cancellationToken = default)
        {
            string path = string.Format(StartLevelPath, levelId);

            using (UnityWebRequest request = UnityWebRequest.Post(_serverConfig.BaseUrl + path, string.Empty, "application/json"))
            {
                string json = await SendAsync(request, cancellationToken);
                StartLevelResponse response = JsonUtility.FromJson<StartLevelResponse>(json);

                if (response == null || string.IsNullOrEmpty(response.sessionId))
                {
                    throw new Exception("Start level response is empty.");
                }

                return response;
            }
        }

        public async UniTask<CompleteLevelResponse> CompleteLevel(int levelId, string sessionId,
            IReadOnlyList<Cell> placement, CancellationToken cancellationToken = default)
        {
            SerializedCell[] serializedPlacement = new SerializedCell[placement.Count];
            for (int i = 0; i < placement.Count; i++)
            {
                serializedPlacement[i] = new SerializedCell
                {
                    row = placement[i].Row,
                    column = placement[i].Column
                };
            }

            string body = JsonUtility.ToJson(new CompleteLevelRequest
            {
                sessionId = sessionId,
                placement = serializedPlacement
            });

            string path = string.Format(CompleteLevelPath, levelId);

            using (UnityWebRequest request = UnityWebRequest.Post(_serverConfig.BaseUrl + path, body, "application/json"))
            {
                string json = await SendAsync(request, cancellationToken);
                return JsonUtility.FromJson<CompleteLevelResponse>(json);
            }
        }

        public async UniTask<UseHintResponse> UseHint(int levelId, string sessionId, CancellationToken cancellationToken = default)
        {
            string body = JsonUtility.ToJson(new UseHintRequest { sessionId = sessionId });
            string path = string.Format(UseHintPath, levelId);

            using (UnityWebRequest request = UnityWebRequest.Post(_serverConfig.BaseUrl + path, body, "application/json"))
            {
                string json = await SendAsync(request, cancellationToken);
                return JsonUtility.FromJson<UseHintResponse>(json);
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

        [Serializable]
        private class CompleteLevelRequest
        {
            public string sessionId;
            public SerializedCell[] placement;
        }

        [Serializable]
        private class UseHintRequest
        {
            public string sessionId;
        }
    }
}
