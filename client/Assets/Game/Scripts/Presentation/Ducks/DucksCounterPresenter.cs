using System;
using DuckDoku.Domain;

namespace DuckDoku.Presentation
{
    public class DucksCounterPresenter : IDisposable, ISessionAttachable
    {
        private readonly DucksCounterView _view;

        private BoardState _board;

        public DucksCounterPresenter(DucksCounterView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            _view = view;
        }

        public void Attach(BoardSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            Detach();

            _board = session.Board;
            _board.CellChanged += OnCellChanged;

            UpdateView();
        }

        public void Detach()
        {
            if (_board == null)
            {
                return;
            }

            _board.CellChanged -= OnCellChanged;
            _board = null;
        }

        public void Dispose()
        {
            Detach();
        }

        private void OnCellChanged(int row, int column)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _view.SetCount(_board.Ducks.Count, _board.Size);
        }
    }
}
