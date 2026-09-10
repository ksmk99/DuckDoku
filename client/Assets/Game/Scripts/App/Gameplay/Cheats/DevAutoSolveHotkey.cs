
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
#if UNITY_EDITOR
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _coordinator.DevSolveInstantly();
            }
#endif
        }
    }
}
