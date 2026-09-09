using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using Zenject;

namespace DuckDoku.App
{
    public class PopupService : IPopupService, IInitializable, IDisposable
    {
        private class PendingPopup
        {
            public IPopupHandle Handle;
            public UniTaskCompletionSource CompletionSource;
        }

        private readonly IStateMachine _stateMachine;
        private readonly Queue<PendingPopup> _queue = new Queue<PendingPopup>();

        private PendingPopup _current;

        public PopupService(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Initialize()
        {
            _stateMachine.Changed += OnSceneChanged;
        }

        public void Dispose()
        {
            _stateMachine.Changed -= OnSceneChanged;
        }

        public UniTask Show(IPopupHandle popup)
        {
            if (popup == null)
            {
                throw new ArgumentNullException(nameof(popup));
            }

            if (IsDuplicate(popup))
            {
                popup.Release();
                return UniTask.CompletedTask;
            }

            var pending = new PendingPopup
            {
                Handle = popup,
                CompletionSource = new UniTaskCompletionSource()
            };

            popup.Hide();
            _queue.Enqueue(pending);

            if (_current == null)
            {
                ShowNext();
            }

            return pending.CompletionSource.Task;
        }

        private bool IsDuplicate(IPopupHandle popup)
        {
            Type type = popup.GetType();

            if (_current != null && _current.Handle.GetType() == type)
            {
                return true;
            }

            foreach (PendingPopup pending in _queue)
            {
                if (pending.Handle.GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowNext()
        {
            if (_queue.Count == 0)
            {
                _current = null;
                return;
            }

            _current = _queue.Dequeue();
            _current.Handle.Closed += OnCurrentClosed;
            _current.Handle.Show();
        }

        private void OnCurrentClosed()
        {
            PendingPopup finished = _current;

            finished.Handle.Closed -= OnCurrentClosed;
            _current = null;

            finished.Handle.Release();
            finished.CompletionSource.TrySetResult();

            ShowNext();
        }

        private void OnSceneChanged(ISceneState nextState)
        {
            if (_current != null)
            {
                PendingPopup finished = _current;

                finished.Handle.Closed -= OnCurrentClosed;
                _current = null;

                finished.Handle.Release();
                finished.CompletionSource.TrySetResult();
            }

            foreach (PendingPopup pending in _queue)
            {
                pending.Handle.Release();
                pending.CompletionSource.TrySetResult();
            }

            _queue.Clear();
        }
    }
}
