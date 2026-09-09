using System.Collections.Generic;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class LevelViewPool
    {
        private readonly LevelView _prefab;
        private readonly RectTransform _parent;
        private readonly Stack<LevelView> _free = new Stack<LevelView>();

        public LevelViewPool(LevelView prefab, RectTransform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                LevelView view = Object.Instantiate(_prefab, _parent);
                view.gameObject.SetActive(false);
                _free.Push(view);
            }
        }

        public LevelView Get()
        {
            LevelView view = _free.Count > 0
                ? _free.Pop()
                : Object.Instantiate(_prefab, _parent);

            view.gameObject.SetActive(true);
            return view;
        }

        public void Release(LevelView view)
        {
            view.gameObject.SetActive(false);
            _free.Push(view);
        }
    }
}
