using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace DuckDoku.App
{
    public class BootAuthPresenter : IInitializable, IDisposable
    {
        private readonly IGuestAuthClient _authClient;
        private readonly TMP_Text _output;
        private readonly PlayerSession _session;
        private readonly IProfileClient _profileClient;
        private readonly IServerTimeService _serverTimeService;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        [Inject]
        public BootAuthPresenter(IGuestAuthClient authClient,
            TMP_Text output,
            PlayerSession session,
            IProfileClient profileClient,
            IServerTimeService serverTimeService)
        {
            _authClient = authClient;
            _output = output;
            _session = session;
            _profileClient = profileClient;
            _serverTimeService = serverTimeService;
        }

        public void Initialize()
        {
            LoginAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid LoginAsync(CancellationToken cancellationToken)
        {
            _output.text = "Connecting...";

            try
            {
                if (!_session.IsAuthenticated)
                {
                    await _authClient.AuthenticateAsGuestAsync(cancellationToken);
                }

                PlayerProfile profile = await _profileClient.GetProfile(cancellationToken);
                _session.SetProfile(profile);

                await _serverTimeService.SynchronizeAsync(cancellationToken);

                string name = profile.HasName ? profile.displayName : "no name";

                _output.text = $"Player: {profile.playerId}\nName: {name}\nTime: {_serverTimeService.UtcNow}";

                SceneManager.LoadScene("Gameplay");
            }
            catch (Exception exception)
            {
                _output.text = "Server unavailable";

                Debug.LogError(exception);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts?.Dispose();
        }
    }
}
