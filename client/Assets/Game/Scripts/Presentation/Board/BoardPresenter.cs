using System;
using DuckDoku.Domain;
using UnityEngine;
using Zenject;

namespace DuckDoku.Presentation
{
    public class BoardPresenter : IInitializable, IDisposable, IBoardGestureTarget
    {
        private readonly BoardView _boardView;
        private readonly BoardGesture _gesture;

        private BoardSession _session;

        public BoardPresenter(BoardView boardView)
        {
            if (boardView == null)
            {
                throw new ArgumentNullException(nameof(boardView));
            }

            _boardView = boardView;
            _gesture = new BoardGesture(this);
        }

        public void Initialize()
        {
            _boardView.CellPressed += OnCellPressed;
            _boardView.CellEntered += OnCellEntered;
            _boardView.CellReleased += OnCellReleased;
        }

        public void Dispose()
        {
            _boardView.CellPressed -= OnCellPressed;
            _boardView.CellEntered -= OnCellEntered;
            _boardView.CellReleased -= OnCellReleased;

            DetachBoard();
        }

        public void AttachBoard(BoardSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            DetachBoard();

            _session = session;
            _session.Board.CellChanged += OnCellChanged;

            _boardView.Build(_session.Board.Size, CreateRegionMap(_session.Board));

            RedrawAll();
        }

        public void DetachBoard()
        {
            if (_session == null)
            {
                return;
            }

            _session.Board.CellChanged -= OnCellChanged;
            _session = null;

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
            _session?.TryPlaceDuck(row, column);
        }

        private void OnCellPressed(int pointerId, int row, int column)
        {
            _gesture.Press(pointerId, row, column, Time.unscaledTime);
        }

        private void OnCellEntered(int pointerId, int row, int column)
        {
            _gesture.Enter(pointerId, row, column);
        }

        private void OnCellReleased(int pointerId)
        {
            _gesture.Release(pointerId, Time.unscaledTime);
        }

        private void OnCellChanged(int row, int column)
        {
            _boardView.Show(
                row,
                column,
                _session.Board.GetCellState(row, column),
                _session.Board.HasConflict(row, column));
        }

        private void RedrawAll()
        {
            for (int row = 0; row < _session.Board.Size; row++)
            {
                for (int column = 0; column < _session.Board.Size; column++)
                {
                    OnCellChanged(row, column);
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
