using System;
using UnityEngine;

namespace DuckDoku.App
{
    public class DeviceIdProvider : IDeviceIdProvider
    {
        private const string DeviceIdKey = "DeviceID";

        private string _deviceId = String.Empty;

        public string GetDeviceID()
        {
            if (_deviceId != String.Empty)
            {
                return _deviceId;
            }

            _deviceId = PlayerPrefs.GetString(DeviceIdKey);
            if (_deviceId != String.Empty)
            {
                return _deviceId;
            }

            _deviceId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(DeviceIdKey, _deviceId);
            PlayerPrefs.Save();

            return _deviceId;
        }
    }
}