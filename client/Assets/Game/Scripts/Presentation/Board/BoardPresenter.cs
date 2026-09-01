using System;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Puzzle;
using Zenject;

namespace DuckDoku.Presentation
{
    public class BoardPresenter : IInitializable, IDisposable
    {
        private readonly ILevelSource _levelSource;
        private readonly BoardView _boardView;
        private readonly GamePlayView _gameplayView;
        private readonly GameplaySettings _settings;

        private BoardState _board;

        public BoardPresenter(
            ILevelSource levelSource,
            BoardView boardView,
            GamePlayView gameplayView,
            GameplaySettings settings)
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

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _levelSource = levelSource;
            _boardView = boardView;
            _gameplayView = gameplayView;
            _settings = settings;
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

            PuzzleDefinition definition = await _levelSource.GetPuzzleAsync(_settings.Size, _settings.Difficulty);

            DetachBoard();

            _board = new BoardState(definition);
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
            LoadAsync().Forget();
        }

        private void OnBoardChanged()
        {
            RedrawAll();
        }

        private void OnBoardSolved()
        {
            _gameplayView.ShowVictory(true);
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
