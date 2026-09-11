using System;
using DuckDoku.Domain;
using DuckDoku.Puzzle;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class BoardPresenter : IInitializable, IDisposable, ITickable, IBoardGestureTarget, IBoardEditability, ISessionAttachable
    {
        private readonly BoardView _boardView;
        private readonly CellFeedbackConfig _feedback;
        private readonly BoardInputConfig _inputConfig;
        private readonly HintService _hintService;
        private readonly ISfxPlayer _sfxPlayer;
        private readonly BoardGesture _gesture;

        private BoardSession _session;
        private Action<Cell[]> _onSolved;
        private Cell? _highlightedHintCell;

        public BoardPresenter(
            BoardView boardView,
            CellFeedbackConfig feedback,
            BoardInputConfig inputConfig,
            HintService hintService,
            ISfxPlayer sfxPlayer)
        {
            if (boardView == null)
            {
                throw new ArgumentNullException(nameof(boardView));
            }

            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }

            if (inputConfig == null)
            {
                throw new ArgumentNullException(nameof(inputConfig));
            }

            if (hintService == null)
            {
                throw new ArgumentNullException(nameof(hintService));
            }

            if (sfxPlayer == null)
            {
                throw new ArgumentNullException(nameof(sfxPlayer));
            }

            _boardView = boardView;
            _feedback = feedback;
            _inputConfig = inputConfig;
            _hintService = hintService;
            _sfxPlayer = sfxPlayer;
            _gesture = new BoardGesture(this, _inputConfig.DoubleTapSeconds, _inputConfig.StaleGestureTimeoutSeconds);
        }

        public void Initialize()
        {
            _boardView.CellPressed += OnCellPressed;
            _boardView.CellEntered += OnCellEntered;
            _boardView.CellReleased += OnCellReleased;

            _hintService.Changed += OnHintChanged;
        }

        public void Dispose()
        {
            _boardView.CellPressed -= OnCellPressed;
            _boardView.CellEntered -= OnCellEntered;
            _boardView.CellReleased -= OnCellReleased;

            _hintService.Changed -= OnHintChanged;

            Detach();
        }

        public void Attach(BoardSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            Detach();

            _session = session;
            _session.Board.CellChanged += OnCellChanged;
            _session.DuckRejected += OnDuckRejected;
            
            int size = session.Board.Size;
            _onSolved = _ => PlaySolvedWave(size);
            _session.Board.Solved += _onSolved;

            _boardView.Build(_session.Board.Size, CreateRegionMap(_session.Board), _feedback);

            RedrawAll();
        }

        public void Detach()
        {
            if (_session == null)
            {
                return;
            }

            _session.Board.CellChanged -= OnCellChanged;
            _session.DuckRejected -= OnDuckRejected;
            _session.Board.Solved -= _onSolved;
            _onSolved = null;

            _session = null;
            _highlightedHintCell = null;

            _gesture.Reset();
        }

        public bool CanEdit(int row, int column)
        {
            return _session != null && _session.Board.CanEdit(row, column);
        }

        public bool IsMarked(int row, int column)
        {
            return _session != null && _session.Board.GetCellState(row, column) == CellState.Cross;
        }

        public void Mark(int row, int column, bool marked)
        {
            _session?.SetMark(row, column, marked);
        }

        public void PlaceDuck(int row, int column)
        {
            if (_session == null)
            {
                return;
            }

            if (!_session.Board.CanEdit(row, column))
            {
                _boardView.PlayBlocked(row, column);
                return;
            }

            BoardState board = _session.Board;

            if (_session.TryPlaceDuck(row, column))
            {
                PlayCorrectPlacementFeedback(row, column, board);
            }
        }

        public void Tick()
        {
            _gesture.Tick(Time.unscaledTime);
        }

        private void OnCellPressed(int pointerId, int row, int column)
        {
            _gesture.Press(pointerId, row, column, Time.unscaledTime);
        }

        private void OnCellEntered(int pointerId, int row, int column)
        {
            _gesture.Enter(pointerId, row, column, Time.unscaledTime);
        }

        private void OnCellReleased(int pointerId)
        {
            _gesture.Release(pointerId, Time.unscaledTime);
        }

        private void OnCellChanged(int row, int column)
        {
            CellState state = ApplyCellState(row, column);

            if (state == CellState.Cross)
            {
                _sfxPlayer.Play(SfxId.CrossPlaced);
            }
            else if (state == CellState.Empty)
            {
                _sfxPlayer.Play(SfxId.CrossRemoved);
            }
        }

        private CellState ApplyCellState(int row, int column)
        {
            CellState state = _session.Board.GetCellState(row, column);

            _boardView.Show(row, column, state, _session.Board.HasConflict(row, column));

            if (state == CellState.Cross)
            {
                _boardView.PlayCrossPainted(row, column);
            }

            return state;
        }

        private void OnHintChanged()
        {
            if (_highlightedHintCell.HasValue)
            {
                Cell previous = _highlightedHintCell.Value;
                _boardView.PlayHintAccentClear(previous.Row, previous.Column);
                _highlightedHintCell = null;
            }

            Cell? hint = _hintService.ActiveHint;

            if (hint == null)
            {
                return;
            }

            _highlightedHintCell = hint.Value;
            _boardView.PlayHintAccent(hint.Value.Row, hint.Value.Column);
        }

        private void OnDuckRejected(Cell cell)
        {
            _boardView.PlayWrongPlacement(cell.Row, cell.Column);
            _sfxPlayer.Play(SfxId.WrongPlacement);
        }

        private void PlayCorrectPlacementFeedback(int row, int column, BoardState board)
        {
            _boardView.PlayCorrectPlacement(row, column);
            _sfxPlayer.Play(SfxId.DuckPlaced);

            if (board.IsSolved)
            {
                return;
            }

            PlayWave(row, column, board.Size, skipOrigin: true);
        }

        private void PlaySolvedWave(int size)
        {
            int centerRow = (size - 1) / 2;
            int centerColumn = (size - 1) / 2;

            PlayWave(centerRow, centerColumn, size, skipOrigin: false);
        }

        private void PlayWave(int originRow, int originColumn, int size, bool skipOrigin)
        {
            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    if (skipOrigin && row == originRow && column == originColumn)
                    {
                        continue;
                    }

                    float delay = _feedback.GroupWaveDelayStep * ChebyshevDistance(row, column, originRow, originColumn);
                    _boardView.PlayGroupWave(row, column, delay);
                }
            }
        }
        
        private static int ChebyshevDistance(int rowA, int columnA, int rowB, int columnB)
        {
            return Mathf.Max(Mathf.Abs(rowA - rowB), Mathf.Abs(columnA - columnB));
        }

        private void RedrawAll()
        {
            for (int row = 0; row < _session.Board.Size; row++)
            {
                for (int column = 0; column < _session.Board.Size; column++)
                {
                    ApplyCellState(row, column);
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
