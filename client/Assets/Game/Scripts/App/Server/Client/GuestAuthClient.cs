using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class GuestAuthClient : IGuestAuthClient
    {
        private const string GuestAuthPath = "/api/v1/auth/guest";

        private readonly IApiRequestExecutor _api;
        private readonly IDeviceIdProvider _deviceIdProvider;
        private readonly PlayerSession _session;

        public GuestAuthClient(IApiRequestExecutor api,
            IDeviceIdProvider deviceIdProvider,
            PlayerSession session)
        {
            _api = api;
            _deviceIdProvider = deviceIdProvider;
            _session = session;
        }

        public async UniTask AuthenticateAsGuestAsync(CancellationToken cancellationToken = default)
        {
            GuestRequest payload = new GuestRequest
            {
                deviceId = _deviceIdProvider.GetDeviceID()
            };

            GuestResponse response =
                await _api.PostAsync<GuestResponse>(GuestAuthPath, payload, cancellationToken, requireAuth: false);

            if (response == null || string.IsNullOrEmpty(response.playerId))
            {
                throw new Exception("Guest login returned an empty response.");
            }

            _session.SetAuthentication(response.playerId, response.token);
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
