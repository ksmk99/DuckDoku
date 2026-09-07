using System;
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
        private readonly BoardPresenter _boardPresenter;
        private readonly GamePlayView _gameplayView;

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
            BoardPresenter boardPresenter,
            GamePlayView gameplayView)
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

            if (boardPresenter == null)
            {
                throw new ArgumentNullException(nameof(boardPresenter));
            }

            if (gameplayView == null)
            {
                throw new ArgumentNullException(nameof(gameplayView));
            }

            _levelSource = levelSource;
            _levelLauncher = levelLauncher;
            _levelSessionService = levelSessionService;
            _sessionFactory = sessionFactory;
            _stateMachine = stateMachine;
            _finishContext = finishContext;
            _boardPresenter = boardPresenter;
            _gameplayView = gameplayView;
        }

        public void Initialize()
        {
            _finishContext.RetryRequested += OnRetryRequested;
            _gameplayView.NextRequested += OnNextRequested;

            StartAsync().Forget();
        }

        public void Dispose()
        {
            _finishContext.RetryRequested -= OnRetryRequested;
            _gameplayView.NextRequested -= OnNextRequested;

            DetachSession();
        }

        private void OnNextRequested()
        {
            _levelLauncher.ReturnToMap();
        }

        private async UniTaskVoid StartAsync()
        {
            _gameplayView.SetNextEnabled(false);

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

            BeginAttempt();
        }

        private void BeginAttempt()
        {
            DetachSession();

            _session = _sessionFactory.Create(_puzzle);
            _session.Board.Solved += OnSolved;
            _session.Mistakes.Failed += OnFailed;
            _session.Mistakes.MistakeMade += OnMistakeMade;

            _boardPresenter.AttachBoard(_session);
            _gameplayView.SetLivesRemaining(MistakeTracker.MaxMistakes);
            _gameplayView.SetNextEnabled(true);
        }

        private void DetachSession()
        {
            if (_session == null)
            {
                return;
            }

            _session.Board.Solved -= OnSolved;
            _session.Mistakes.Failed -= OnFailed;
            _session.Mistakes.MistakeMade -= OnMistakeMade;

            _boardPresenter.DetachBoard();

            _session.Dispose();
            _session = null;
        }

        private void OnMistakeMade(int mistakesLeft)
        {
            _gameplayView.SetLivesRemaining(mistakesLeft);
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
            LevelResult result = default;

            try
            {
                result = await _levelSessionService.CompleteLevelAsync(_levelId, _sessionId, cells);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            _finishContext.SetResult(LevelOutcome.Victory, result.Stars, result.DurationMs);

            ShowFinishAsync().Forget();
        }

        private void OnFailed()
        {
            DetachSession();

            _finishContext.SetResult(LevelOutcome.Defeat, 0, 0);

            ShowFinishAsync().Forget();
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
