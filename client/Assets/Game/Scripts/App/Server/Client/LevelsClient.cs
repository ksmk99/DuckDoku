using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    public class LevelsClient : ILevelsClient
    {
        private const string NextLevelPath = "/api/v1/levels/next";
        private const string StartLevelPath = "/api/v1/levels/{0}/start";
        private const string CompleteLevelPath = "/api/v1/levels/{0}/complete";
        private const string UseHintPath = "/api/v1/levels/{0}/hint";

        private readonly IApiRequestExecutor _api;
        private readonly IIdempotentApiClient _idempotentApi;

        public LevelsClient(IApiRequestExecutor api, IIdempotentApiClient _idempotentApi)
        {
            _api = api;
            this._idempotentApi = _idempotentApi;
        }

        public UniTask<NextLevelResponse> GetNextLevel(CancellationToken cancellationToken = default)
        {
            return _api.GetAsync<NextLevelResponse>(NextLevelPath, cancellationToken);
        }

        public async UniTask<StartLevelResponse> StartLevel(int levelId, CancellationToken cancellationToken = default)
        {
            string path = string.Format(StartLevelPath, levelId);

            StartLevelResponse response = await _idempotentApi.PostIdempotentAsync<StartLevelRequest, StartLevelResponse>(
                path,
                "StartLevel",
                requestId => new StartLevelRequest { requestId = requestId.ToString() },
                cancellationToken);

            if (response == null || string.IsNullOrEmpty(response.sessionId))
            {
                throw new Exception("Start level response is empty.");
            }

            return response;
        }

        public UniTask<CompleteLevelResponse> CompleteLevel(int levelId, string sessionId,
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

            CompleteLevelRequest body = new CompleteLevelRequest
            {
                sessionId = sessionId,
                placement = serializedPlacement
            };

            string path = string.Format(CompleteLevelPath, levelId);

            return _api.PostAsync<CompleteLevelResponse>(path, body, cancellationToken);
        }

        public UniTask<UseHintResponse> UseHint(int levelId, string sessionId,
            CancellationToken cancellationToken = default)
        {
            string path = string.Format(UseHintPath, levelId);
            
            return _idempotentApi.PostIdempotentAsync<UseHintRequest, UseHintResponse>(
                path,
                "UseHint",
                requestId => new UseHintRequest{ sessionId = sessionId, requestId = requestId.ToString() },
                cancellationToken);
        }

        [Serializable]
        private class StartLevelRequest : IIdempotentRequest
        {
            public string requestId;

            public Guid RequestId => Guid.Parse(requestId);
        }

        [Serializable]
        private class CompleteLevelRequest
        {
            public string sessionId;
            public SerializedCell[] placement;
        }

        [Serializable]
        private class UseHintRequest : IIdempotentRequest
        {
            public string sessionId;
            public string requestId;
            
            public Guid RequestId => Guid.Parse(requestId);
        }
    }
}