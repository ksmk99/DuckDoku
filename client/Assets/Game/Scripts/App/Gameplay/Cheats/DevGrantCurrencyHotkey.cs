#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace DuckDoku.App
{
    public class DevGrantCurrencyHotkey : ITickable
    {
        private readonly IDevClient _devClient;
        private readonly ICurrencyService _currencyService;

        public DevGrantCurrencyHotkey(IDevClient devClient, ICurrencyService currencyService)
        {
            _devClient = devClient;
            _currencyService = currencyService;
        }

        public void Tick()
        {
            if (Keyboard.current == null || !Keyboard.current.spaceKey.wasPressedThisFrame)
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
                DevCurrencyResponse response = await _devClient.GrantCurrency();
                _currencyService.Apply(response.balance);
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"DevGrantCurrencyHotkey: failed to grant currency: {exception.Message}");
            }
        }
    }
}
#endif
