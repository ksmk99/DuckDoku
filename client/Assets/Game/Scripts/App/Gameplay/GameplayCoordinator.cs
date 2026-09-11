using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Presentation;
using DuckDoku.Puzzle;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayCoordinator : IInitializable, IDisposable
    {
        private readonly ILevelSource _levelSource;
        private readonly ILevelLauncher _levelLauncher;
        private readonly ILevelSessionService _levelSessionService;
        private readonly IBoardSessionFactory _sessionFactory;
        private readonly IStateMachine _stateMachine;
        private readonly ILevelFinishContext _finishContext;
        private readonly List<ISessionAttachable> _sessionAttachables;
        private readonly HintFlowCoordinator _hintFlowCoordinator;
        private readonly ILevelStartSignal _startSignal;
        private readonly ExitLevelView _exitLevelView;

        private BoardSession _session;
        private PuzzleDefinition _puzzle;
        private int _levelId;
        private string _sessionId;
        private bool _isRetrying;

        public GameplayCoordinator(
            ILevelSource levelSource,
            ILevelLauncher levelLauncher,
            ILevelSessionService levelSessionService,
            IBoardSessionFactory sessionFactory,
            IStateMachine stateMachine,
            ILevelFinishContext finishContext,
            List<ISessionAttachable> sessionAttachables,
            HintFlowCoordinator hintFlowCoordinator,
            ILevelStartSignal startSignal,
            ExitLevelView exitLevelView)
        {
            if (levelSource == null)
            {
                throw new ArgumentNullException(nameof(levelSource));
            }

            if (levelLauncher == null)
            {
                throw new ArgumentNullException(nameof(levelLauncher));
            }

            if (levelSessionService == null)
            {
                throw new ArgumentNullException(nameof(levelSessionService));
            }

            if (sessionFactory == null)
            {
                throw new ArgumentNullException(nameof(sessionFactory));
            }

            if (stateMachine == null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            if (finishContext == null)
            {
                throw new ArgumentNullException(nameof(finishContext));
            }

            if (sessionAttachables == null)
            {
                throw new ArgumentNullException(nameof(sessionAttachables));
            }

            if (hintFlowCoordinator == null)
            {
                throw new ArgumentNullException(nameof(hintFlowCoordinator));
            }

            if (startSignal == null)
            {
                throw new ArgumentNullException(nameof(startSignal));
            }

            if (exitLevelView == null)
            {
                throw new ArgumentNullException(nameof(exitLevelView));
            }

            _levelSource = levelSource;
            _levelLauncher = levelLauncher;
            _levelSessionService = levelSessionService;
            _sessionFactory = sessionFactory;
            _stateMachine = stateMachine;
            _finishContext = finishContext;
            _sessionAttachables = sessionAttachables;
            _hintFlowCoordinator = hintFlowCoordinator;
            _startSignal = startSignal;
            _exitLevelView = exitLevelView;
        }

        public void Initialize()
        {
            _finishContext.RetryRequested += OnRetryRequested;
            _exitLevelView.ExitRequested += OnExitRequested;

            StartAsync().Forget();
        }

        public void Dispose()
        {
            _finishContext.RetryRequested -= OnRetryRequested;
            _exitLevelView.ExitRequested -= OnExitRequested;

            DetachSession();
        }

        private async UniTaskVoid StartAsync()
        {
            _levelId = _levelLauncher.GetLevelId();

            try
            {
                _sessionId = await _levelSessionService.StartLevelAsync(_levelId);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                _levelLauncher.ReturnToMap();
                return;
            }

            LevelToPlay levelToPlay = await _levelSource.GetPuzzleByLevel(_levelId);
            _puzzle = levelToPlay.Puzzle;

            _startSignal.MarkStarted();

            BeginAttempt();
        }

        private void BeginAttempt()
        {
            DetachSession();

            _session = _sessionFactory.Create(_puzzle);
            _session.Board.Solved += OnSolved;
            _session.Mistakes.Failed += OnFailed;

            foreach (ISessionAttachable attachable in _sessionAttachables)
            {
                attachable.Attach(_session);
            }

            _hintFlowCoordinator.Attach(_levelId, _sessionId);
        }

        private void DetachSession()
        {
            if (_session == null)
            {
                return;
            }

            _session.Board.Solved -= OnSolved;
            _session.Mistakes.Failed -= OnFailed;

            foreach (ISessionAttachable attachable in _sessionAttachables)
            {
                attachable.Detach();
            }

            _session.Dispose();
            _session = null;
        }

#if UNITY_EDITOR
        public void DevSolveInstantly()
        {
            if (_session == null)
            {
                return;
            }

            foreach (Cell cell in _puzzle.Solution)
            {
                _session.TryPlaceDuck(cell.Row, cell.Column);
            }
        }
#endif

        private void OnSolved(Cell[] cells)
        {
            DetachSession();

            CompleteAsync(cells).Forget();
        }

        private async UniTaskVoid CompleteAsync(Cell[] cells)
        {
            LevelResult result;

            try
            {
                result = await _levelSessionService.CompleteLevelAsync(_levelId, _sessionId, cells);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                _levelLauncher.ReturnToMap();
                return;
            }

            _finishContext.SetResult(LevelOutcome.Victory, result.Stars, result.DurationMs, result.Coins);

            ShowFinishAsync().Forget();
        }

        private void OnFailed()
        {
            DetachSession();

            _finishContext.SetResult(LevelOutcome.Defeat, 0, 0, 0);

            ShowFinishAsync().Forget();
        }

        private void OnExitRequested()
        {
            if (_session == null)
            {
                return;
            }

            DetachSession();

            _levelLauncher.ReturnToMap();
        }

        private async UniTaskVoid ShowFinishAsync()
        {
            await _stateMachine.LoadAdditive(new LevelFinishSceneState());
        }

        private void OnRetryRequested()
        {
            if (_isRetrying)
            {
                return;
            }

            RetryAsync().Forget();
        }

        private async UniTaskVoid RetryAsync()
        {
            _isRetrying = true;

            await _stateMachine.UnloadAdditive(new LevelFinishSceneState());

            BeginAttempt();

            _isRetrying = false;
        }
    }
}
