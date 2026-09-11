using System;
using DuckDoku.Domain;

namespace DuckDoku.Presentation
{
    public class LivesPresenter : IDisposable, ISessionAttachable
    {
        private readonly LivesView _view;

        private MistakeTracker _mistakes;

        public LivesPresenter(LivesView view)
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

            _mistakes = session.Mistakes;
            _mistakes.MistakeMade += OnMistakeMade;

            _view.ShowFull();
        }

        public void Detach()
        {
            if (_mistakes == null)
            {
                return;
            }

            _mistakes.MistakeMade -= OnMistakeMade;
            _mistakes = null;
        }

        public void Dispose()
        {
            Detach();
        }

        private void OnMistakeMade(int mistakesLeft)
        {
            int lostIndex = MistakeTracker.MaxMistakes - mistakesLeft - 1;

            _view.PlayLost(lostIndex);
        }
    }
}
