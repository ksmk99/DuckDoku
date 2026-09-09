using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DuckDoku.Presentation
{
    public class FrameToggle<TFrame> : IInitializable, IDisposable
        where TFrame : IMetaFrame
    {
        private const int ClickCooldownMs = 200;

        private readonly Button _openBtn;
        private readonly Button _closeBtn;
        private readonly GameObject _frameRoot;
        private readonly TFrame _frame;
        private readonly ClickCooldown _cooldown = new ClickCooldown(ClickCooldownMs);

        public FrameToggle(Button openBtn, Button closeBtn, GameObject frameRoot, TFrame frame)
        {
            _openBtn = openBtn;
            _closeBtn = closeBtn;
            _frameRoot = frameRoot;
            _frame = frame;
        }

        public void Initialize()
        {
            _openBtn.onClick.AddListener(OpenFrame);
            _closeBtn.onClick.AddListener(CloseFrame);
        }

        public void Dispose()
        {
            _openBtn.onClick.RemoveListener(OpenFrame);
            _closeBtn.onClick.RemoveListener(CloseFrame);
        }

        private void OpenFrame()
        {
            if (!_cooldown.TryConsume())
            {
                return;
            }

            _frameRoot.SetActive(true);
            _frame.OpenFrame();
        }

        private void CloseFrame()
        {
            if (!_cooldown.TryConsume())
            {
                return;
            }

            _frameRoot.SetActive(false);
            _frame.CloseFrame();
        }
    }
}
