using DuckDoku.Presentation;
using UnityEngine;

namespace DuckDoku.App
{
    public class AudioSettings : IAudioSettings
    {
        private const string SfxEnabledKey = "Audio.SfxEnabled";

        private bool? _sfxEnabled;

        public bool SfxEnabled
        {
            get
            {
                if (!_sfxEnabled.HasValue)
                {
                    _sfxEnabled = PlayerPrefs.GetInt(SfxEnabledKey, 1) != 0;
                }

                return _sfxEnabled.Value;
            }
        }

        public void SetSfxEnabled(bool enabled)
        {
            _sfxEnabled = enabled;

            PlayerPrefs.SetInt(SfxEnabledKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
