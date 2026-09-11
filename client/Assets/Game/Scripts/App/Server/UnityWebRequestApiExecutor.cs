using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DuckDoku.App
{
    public class UnityWebRequestApiExecutor : IApiRequestExecutor
    {
        private const string AuthorizationHeader = "Authorization";
        private const string AuthorizationScheme = "Ducky ";
        private const string JsonContentType = "application/json";

        private readonly ServerConfig _serverConfig;
        private readonly PlayerSession _session;

        public UnityWebRequestApiExecutor(ServerConfig serverConfig, PlayerSession session)
        {
            _serverConfig = serverConfig;
            _session = session;
        }

        public UniTask<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken,
            bool requireAuth = true)
        {
            return SendAsync<TResponse>(UnityWebRequest.Get(_serverConfig.BaseUrl + path), requireAuth,
                cancellationToken);
        }

        public UniTask<TResponse> PostAsync<TResponse>(string path, object body, CancellationToken cancellationToken,
            bool requireAuth = true)
        {
            UnityWebRequest request =
                UnityWebRequest.Post(_serverConfig.BaseUrl + path, ToJson(body), JsonContentType);

            return SendAsync<TResponse>(request, requireAuth, cancellationToken);
        }

        public UniTask<TResponse> PutAsync<TResponse>(string path, object body, CancellationToken cancellationToken,
            bool requireAuth = true)
        {
            UnityWebRequest request = UnityWebRequest.Put(_serverConfig.BaseUrl + path, ToJson(body));
            request.SetRequestHeader("Content-Type", JsonContentType);

            return SendAsync<TResponse>(request, requireAuth, cancellationToken);
        }

        private async UniTask<TResponse> SendAsync<TResponse>(UnityWebRequest request, bool requireAuth,
            CancellationToken cancellationToken)
        {
            using (request)
            {
                if (requireAuth)
                {
                    if (!_session.IsAuthenticated)
                    {
                        throw new InvalidOperationException("Not authenticated: call guest login first.");
                    }

                    request.SetRequestHeader(AuthorizationHeader, AuthorizationScheme + _session.Token);
                }

                request.timeout = _serverConfig.RequestTimeoutSeconds;

                UnityWebRequest webRequest = await request.SendWebRequest().WithCancellation(cancellationToken);

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                    {
                        return JsonUtility.FromJson<TResponse>(webRequest.downloadHandler.text);
                    }
                    case UnityWebRequest.Result.ProtocolError:
                    {
                        ApiProblemDetails problem =
                            JsonUtility.FromJson<ApiProblemDetails>(webRequest.downloadHandler.text);

                        throw new ApiErrorException((int)webRequest.responseCode, problem?.code,
                            problem?.title ?? webRequest.error);
                    }
                    default:
                    {
                        throw new Exception(webRequest.error);
                    }
                }
            }
        }

        private static string ToJson(object body)
        {
            return body == null ? string.Empty : JsonUtility.ToJson(body);
        }
    }
}