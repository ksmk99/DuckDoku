using System;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Puzzle;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class BoardPresenter : IInitializable, IDisposable
    {
        private readonly ILevelSource _levelSource;
        private readonly BoardView _boardView;
        private readonly GamePlayView _gameplayView;
        private readonly ILevelLauncher _levelLauncher;
        private readonly ILevelSessionService _levelSessionService;

        private BoardState _board;
        private int _levelId;
        private string _sessionId;

        public BoardPresenter(
            ILevelSource levelSource,
            BoardView boardView,
            GamePlayView gameplayView,
            ILevelLauncher levelLauncher,
            ILevelSessionService levelSessionService)
        {
            if (levelSource == null)
            {
                throw new ArgumentNullException(nameof(levelSource));
            }

            if (boardView == null)
            {
                throw new ArgumentNullException(nameof(boardView));
            }

            if (gameplayView == null)
            {
                throw new ArgumentNullException(nameof(gameplayView));
            }

            if (levelLauncher == null)
            {
                throw new ArgumentNullException(nameof(levelLauncher));
            }

            if (levelSessionService == null)
            {
                throw new ArgumentNullException(nameof(levelSessionService));
            }

            _levelSource = levelSource;
            _boardView = boardView;
            _gameplayView = gameplayView;
            _levelLauncher = levelLauncher;
            _levelSessionService = levelSessionService;
        }

        public void Initialize()
        {
            _boardView.CellClicked += OnCellClicked;
            _gameplayView.NextRequested += OnNextRequested;

            LoadAsync().Forget();
        }

        public void Dispose()
        {
            _boardView.CellClicked -= OnCellClicked;
            _gameplayView.NextRequested -= OnNextRequested;

            DetachBoard();
        }

        private async UniTaskVoid LoadAsync()
        {
            _gameplayView.SetNextEnabled(false);
            _gameplayView.ShowVictory(false);

            int levelId = _levelLauncher.GetLevelId();

            try
            {
                _sessionId = await _levelSessionService.StartLevelAsync(levelId);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                _levelLauncher.ReturnToMap();
                return;
            }

            LevelToPlay levelToPlay = await _levelSource.GetPuzzleByLevel(levelId);

            DetachBoard();

            _levelId = levelToPlay.LevelId;
            _board = new BoardState(levelToPlay.Puzzle);
            _board.Changed += OnBoardChanged;
            _board.Solved += OnBoardSolved;

            _boardView.Build(_board.Size, CreateRegionMap(_board));

            RedrawAll();

            _gameplayView.SetNextEnabled(true);
        }

        private void DetachBoard()
        {
            if (_board == null)
            {
                return;
            }

            _board.Changed -= OnBoardChanged;
            _board.Solved -= OnBoardSolved;
            _board = null;
        }

        private void OnCellClicked(int row, int column)
        {
            _board?.Toggle(row, column);
        }

        private void OnNextRequested()
        {
            _levelLauncher.ReturnToMap();
        }

        private void OnBoardChanged()
        {
            RedrawAll();
        }

        private void OnBoardSolved(Cell[] cells)
        {
            CompleteAsync(cells).Forget();
        }

        private async UniTaskVoid CompleteAsync(Cell[] cells)
        {
            try
            {
                await _levelSessionService.CompleteLevelAsync(_levelId, _sessionId, cells);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            _levelLauncher.ReturnToMap();
        }

        private void RedrawAll()
        {
            for (int row = 0; row < _board.Size; row++)
            {
                for (int column = 0; column < _board.Size; column++)
                {
                    _boardView.Show(
                        row,
                        column,
                        _board.GetCellState(row, column),
                        _board.HasConflict(row, column));
                }
            }
        }

        private static int[] CreateRegionMap(BoardState board)
        {
            int[] regions = new int[board.Size * board.Size];

            for (int row = 0; row < board.Size; row++)
            {
                for (int column = 0; column < board.Size; column++)
                {
                    regions[row * board.Size + column] = board.RegionAt(row, column);
                }
            }

            return regions;
        }
    }
}
