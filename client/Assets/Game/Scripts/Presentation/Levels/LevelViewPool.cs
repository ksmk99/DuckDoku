using System.Collections.Generic;

namespace DuckDoku.Presentation
{
    public class LevelViewPool
    {
        private readonly LevelView.Factory _factory;
        private readonly Stack<LevelView> _free = new Stack<LevelView>();

        public LevelViewPool(LevelView.Factory factory)
        {
            _factory = factory;
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                LevelView view = _factory.Create();
                view.gameObject.SetActive(false);
                _free.Push(view);
            }
        }

        public LevelView Get()
        {
            LevelView view = _free.Count > 0
                ? _free.Pop()
                : _factory.Create();

            view.gameObject.SetActive(true);
            return view;
        }

        public void Release(LevelView view)
        {
            if (view == null)
            {
                return;
            }

            view.gameObject.SetActive(false);
            _free.Push(view);
        }
    }
}
