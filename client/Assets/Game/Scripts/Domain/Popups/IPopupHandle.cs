using System;

namespace DuckDoku.Domain
{
    public interface IPopupHandle
    {
        event Action Closed;

        void Show();
        void Hide();
        void Release();
    }
}
