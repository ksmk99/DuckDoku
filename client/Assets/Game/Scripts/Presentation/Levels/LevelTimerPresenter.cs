using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class LevelTimerPresenter : IInitializable, IDisposable
    {
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

        private readonly LevelTimerView _view;
        private readonly ILevelStartSignal _startSignal;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private float _startTime;

        public LevelTimerPresenter(LevelTimerView view, ILevelStartSignal startSignal)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (startSignal == null)
            {
                throw new ArgumentNullException(nameof(startSignal));
            }

            _view = view;
            _startSignal = startSignal;
        }

        public void Initialize()
        {
            _startSignal.Started += OnStarted;

            if (_startSignal.HasStarted)
            {
                OnStarted();
            }
        }

        public void Dispose()
        {
            _startSignal.Started -= OnStarted;

            _cts.Cancel();
            _cts.Dispose();
        }

        private void OnStarted()
        {
            _startTime = Time.unscaledTime;
            _view.SetElapsed(TimeSpan.Zero);

            TickAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid TickAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TickInterval, cancellationToken: cancellationToken).SuppressCancellationThrow();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                float elapsed = Time.unscaledTime - _startTime;
                _view.SetElapsed(TimeSpan.FromSeconds(elapsed));
            }
        }
    }
}
