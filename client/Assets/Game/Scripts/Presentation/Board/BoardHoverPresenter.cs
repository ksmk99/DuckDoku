using System;
using Zenject;

namespace DuckDoku.Presentation
{
    public class BoardHoverPresenter : IInitializable, IDisposable
    {
        private readonly BoardView _boardView;
        private readonly IBoardEditability _editability;

        public BoardHoverPresenter(BoardView boardView, IBoardEditability editability)
        {
            if (boardView == null)
            {
                throw new ArgumentNullException(nameof(boardView));
            }

            if (editability == null)
            {
                throw new ArgumentNullException(nameof(editability));
            }

            _boardView = boardView;
            _editability = editability;
        }

        public void Initialize()
        {
            _boardView.CellHoverEntered += OnCellHoverEntered;
            _boardView.CellHoverExited += OnCellHoverExited;
        }

        public void Dispose()
        {
            _boardView.CellHoverEntered -= OnCellHoverEntered;
            _boardView.CellHoverExited -= OnCellHoverExited;
        }

        private void OnCellHoverEntered(int row, int column)
        {
            if (!_editability.CanEdit(row, column))
            {
                return;
            }

            _boardView.PlayHoverEnter(row, column);
        }

        private void OnCellHoverExited(int row, int column)
        {
            _boardView.PlayHoverExit(row, column);
        }
    }
}
