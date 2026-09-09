using System;

namespace DuckDoku.Presentation
{
    public class BoardGesture
    {
        private readonly IBoardGestureTarget _target;
        private readonly float _doubleTapSeconds;
        private readonly float _staleTimeoutSeconds;

        private bool _isActive;
        private int _pointerId;
        private float _lastActivityTime;

        private bool _isStartPainted;
        private bool _isDoubleTapPending;
        private bool _hasMoved;

        private int _startRow;
        private int _startColumn;
        private int _lastRow;
        private int _lastColumn;

        private bool _hasPaintMode;
        private bool _paintMarked;

        private int _tapRow = -1;
        private int _tapColumn = -1;
        private float _tapTime = float.NegativeInfinity;

        public BoardGesture(IBoardGestureTarget target, float doubleTapSeconds, float staleTimeoutSeconds)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            _target = target;
            _doubleTapSeconds = doubleTapSeconds;
            _staleTimeoutSeconds = staleTimeoutSeconds;
        }

        public void Press(int pointerId, int row, int column, float time)
        {
            if (_isActive)
            {
                return;
            }

            _isActive = true;
            _pointerId = pointerId;
            _lastActivityTime = time;

            _hasMoved = false;
            _hasPaintMode = false;

            _startRow = row;
            _startColumn = column;
            _lastRow = row;
            _lastColumn = column;

            _isDoubleTapPending = row == _tapRow
                                  && column == _tapColumn
                                  && time - _tapTime <= _doubleTapSeconds;

            _isStartPainted = false;

            if (_isDoubleTapPending)
            {
                return;
            }

            Paint(row, column);

            _isStartPainted = true;
        }

        public void Enter(int pointerId, int row, int column, float time = 0f)
        {
            if (!_isActive || pointerId != _pointerId)
            {
                return;
            }

            if (row == _lastRow && column == _lastColumn)
            {
                return;
            }

            _lastActivityTime = time;
            _hasMoved = true;
            _isDoubleTapPending = false;

            if (!_isStartPainted)
            {
                Paint(_startRow, _startColumn);
                _isStartPainted = true;
            }

            PaintLineTo(row, column);
        }

        public void Release(int pointerId, float time)
        {
            if (!_isActive || pointerId != _pointerId)
            {
                return;
            }

            _isActive = false;

            if (_hasMoved)
            {
                ForgetTap();
                return;
            }

            if (_isDoubleTapPending)
            {
                _isDoubleTapPending = false;

                ForgetTap();

                _target.PlaceDuck(_startRow, _startColumn);
                return;
            }

            _tapRow = _startRow;
            _tapColumn = _startColumn;
            _tapTime = time;
        }

        public void Reset()
        {
            _isActive = false;
            _isStartPainted = false;
            _isDoubleTapPending = false;
            _hasMoved = false;
            _hasPaintMode = false;

            ForgetTap();
        }

        public void Tick(float time)
        {
            if (!_isActive)
            {
                return;
            }

            if (time - _lastActivityTime < _staleTimeoutSeconds)
            {
                return;
            }

            _isActive = false;
            _isStartPainted = false;
            _isDoubleTapPending = false;
            _hasMoved = false;
            _hasPaintMode = false;

            ForgetTap();
        }

        private void PaintLineTo(int row, int column)
        {
            while (_lastRow != row || _lastColumn != column)
            {
                int rowStep = row - _lastRow;
                int columnStep = column - _lastColumn;

                if (Math.Abs(rowStep) >= Math.Abs(columnStep))
                {
                    _lastRow += rowStep > 0 ? 1 : -1;
                }
                else
                {
                    _lastColumn += columnStep > 0 ? 1 : -1;
                }

                Paint(_lastRow, _lastColumn);
            }
        }

        private void Paint(int row, int column)
        {
            if (!_target.CanEdit(row, column))
            {
                return;
            }

            if (!_hasPaintMode)
            {
                _paintMarked = !_target.IsMarked(row, column);
                _hasPaintMode = true;
            }

            _target.Mark(row, column, _paintMarked);
        }

        private void ForgetTap()
        {
            _tapRow = -1;
            _tapColumn = -1;
            _tapTime = float.NegativeInfinity;
        }
    }
}
