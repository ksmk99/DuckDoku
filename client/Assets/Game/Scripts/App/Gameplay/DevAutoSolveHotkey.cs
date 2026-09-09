#if UNITY_EDITOR
using UnityEngine.InputSystem;
using Zenject;

namespace DuckDoku.App
{
    public class DevAutoSolveHotkey : ITickable
    {
        private readonly GameplayCoordinator _coordinator;

        public DevAutoSolveHotkey(GameplayCoordinator coordinator)
        {
            _coordinator = coordinator;
        }

        public void Tick()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _coordinator.DevSolveInstantly();
            }
        }
    }
}
#endif
