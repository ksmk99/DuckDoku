using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace DuckDoku.App
{
    public class BootAuthPresenter : IInitializable, IDisposable
    {
        private readonly IGuestAuthClient _authClient;
        private readonly BootAuthView _view;
        private readonly BootRetryConfig _retryConfig;
        private readonly PlayerSession _session;
        private readonly IProfileClient _profileClient;
        private readonly IServerTimeService _serverTimeService;
        private readonly IStateMachine _sceneStateMachine;
        private readonly RemoteLoadCoordinator _remoteLoadCoordinator;

        private CancellationTokenSource _cts;

        [Inject]
        public BootAuthPresenter(IGuestAuthClient authClient,
            BootAuthView view,
            BootRetryConfig retryConfig,
            PlayerSession session,
            IProfileClient profileClient,
            IServerTimeService serverTimeService,
            IStateMachine sceneStateMachine,
            RemoteLoadCoordinator remoteLoadCoordinator)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (retryConfig == null)
            {
                throw new ArgumentNullException(nameof(retryConfig));
            }

            if (remoteLoadCoordinator == null)
            {
                throw new ArgumentNullException(nameof(remoteLoadCoordinator));
            }

            _authClient = authClient;
            _view = view;
            _retryConfig = retryConfig;
            _session = session;
            _profileClient = profileClient;
            _serverTimeService = serverTimeService;
            _sceneStateMachine = sceneStateMachine;
            _remoteLoadCoordinator = remoteLoadCoordinator;
        }

        public void Initialize()
        {
            _view.RetryRequested += OnRetryRequested;
            StartLoginFlow();
        }

        public void Dispose()
        {
            _view.RetryRequested -= OnRetryRequested;
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void OnRetryRequested()
        {
            StartLoginFlow();
        }

        private void StartLoginFlow()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            LoginWithRetryAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid LoginWithRetryAsync(CancellationToken cancellationToken)
        {
            float startTime = Time.realtimeSinceStartup;
            int attempt = 0;

            while (true)
            {
                attempt++;
                _view.ShowStatus("Loading...");

                try
                {
                    await LoginOnceAsync(cancellationToken);
                    return;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (UnityWebRequestException networkException)
                {
                    float elapsedAfterFailure = Time.realtimeSinceStartup - startTime;
                    if (elapsedAfterFailure >= _retryConfig.MaxRetryWindowSeconds)
                    {
                        _view.ShowError("Server unavailable. Tap to retry.");
                        Debug.LogError(networkException);
                        return;
                    }

                    float delay = Mathf.Min(
                        _retryConfig.BaseRetryDelaySeconds * Mathf.Pow(2f, attempt - 1),
                        _retryConfig.MaxRetryDelaySeconds);

                    try
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
                catch (Exception exception)
                {
                    _view.ShowError("Server unavailable. Tap to retry.");
                    Debug.LogError(exception);
                    return;
                }
            }
        }

        private async UniTask LoginOnceAsync(CancellationToken cancellationToken)
        {
            if (!_session.IsAuthenticated)
            {
                await _authClient.AuthenticateAsGuestAsync(cancellationToken);
            }

            await _serverTimeService.SynchronizeAsync(cancellationToken);
            
            UniTask<PlayerProfile> profileTask = _profileClient.GetProfile(cancellationToken);
            UniTask remoteLoadTask = _remoteLoadCoordinator.LoadAllAsync(cancellationToken);

            
            PlayerProfile profile = await profileTask;
            await remoteLoadTask;

            _session.SetProfile(profile);

            await _sceneStateMachine.TransitionTo(new MetaSceneState());
        }
    }
}
