using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour, ISfxPlayer
    {
        private SfxConfig _config;
        private IAudioSettings _audioSettings;
        private AudioSource _source;

        [Inject]
        private void Construct(SfxConfig config, IAudioSettings audioSettings)
        {
            _config = config;
            _audioSettings = audioSettings;
        }

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
        }

        public void Play(SfxId id)
        {
            if (!_audioSettings.SfxEnabled)
            {
                return;
            }

            if (!_config.TryGetSound(id, out AudioClip clip, out float volume, out float pitch))
            {
                return;
            }

            _source.pitch = pitch;
            _source.PlayOneShot(clip, volume);
        }
    }
}
