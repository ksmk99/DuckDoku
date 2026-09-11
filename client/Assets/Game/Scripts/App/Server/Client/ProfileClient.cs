using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class ProfileClient : IProfileClient
    {
        private const string ProfilePath = "/api/v1/profile";
        private const string ChangeNamePath = "/api/v1/profile/name";

        private readonly IApiRequestExecutor _api;

        public ProfileClient(IApiRequestExecutor api)
        {
            _api = api;
        }

        public UniTask<PlayerProfile> GetProfile(CancellationToken cancellationToken = default)
        {
            return RequireValidProfile(_api.GetAsync<PlayerProfile>(ProfilePath, cancellationToken));
        }

        public UniTask<PlayerProfile> ChangeName(string name, CancellationToken cancellationToken = default)
        {
            ChangeNameRequest body = new ChangeNameRequest { displayName = name };

            return RequireValidProfile(_api.PutAsync<PlayerProfile>(ChangeNamePath, body, cancellationToken));
        }

        private static async UniTask<PlayerProfile> RequireValidProfile(UniTask<PlayerProfile> request)
        {
            PlayerProfile profile = await request;

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
