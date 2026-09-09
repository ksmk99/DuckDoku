#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace DuckDoku.App
{
    public class DevMaxEnergyHotkey : ITickable
    {
        private readonly IDevClient _devClient;
        private readonly IEnergyService _energyService;

        public DevMaxEnergyHotkey(IDevClient devClient, IEnergyService energyService)
        {
            _devClient = devClient;
            _energyService = energyService;
        }

        public void Tick()
        {
            if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
            {
                return;
            }

            bool ctrlPressed = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
            bool shiftPressed = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

            if (ctrlPressed && shiftPressed)
            {
                GrantAsync().Forget();
            }
        }

        private async UniTaskVoid GrantAsync()
        {
            try
            {
                DevEnergyResponse response = await _devClient.GrantMaxEnergy();
                _energyService.Apply(response.energy, response.energyMax, response.energyRefillMs);
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"DevMaxEnergyHotkey: failed to grant max energy: {exception.Message}");
            }
        }
    }
}
#endif
