#if UNITY_EDITOR
using UnityEngine.InputSystem;
using Zenject;

namespace DuckDoku.App
{
    // Developer Script: только для редактора, ни при каких условиях не попадает в билд игрока
    // (весь файл под UNITY_EDITOR). Позволяет мгновенно решить текущий уровень пробелом,
    // не выходя из PlayMode.
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
