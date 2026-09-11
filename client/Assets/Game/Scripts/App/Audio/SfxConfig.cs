using System;
using System.Collections.Generic;
using DuckDoku.Presentation;
using UnityEngine;

namespace DuckDoku.App
{
    [CreateAssetMenu(fileName = "SfxConfig", menuName = "DuckDoku/Sfx Config")]
    public class SfxConfig : ScriptableObject
    {
        [Serializable]
        private class SfxEntry
        {
            public SfxId Id;
            public AudioClip Clip;
            [Range(0f, 1f)] public float Volume = 1f;
            public float PitchMin = 1f;
            public float PitchMax = 1f;
        }

        [SerializeField] private List<SfxEntry> _entries = new List<SfxEntry>();

        private Dictionary<SfxId, SfxEntry> _lookup;

        public bool TryGetSound(SfxId id, out AudioClip clip, out float volume, out float pitch)
        {
            BuildLookupIfNeeded();

            if (_lookup.TryGetValue(id, out SfxEntry entry) && entry.Clip != null)
            {
                clip = entry.Clip;
                volume = entry.Volume;
                pitch = UnityEngine.Random.Range(entry.PitchMin, entry.PitchMax);
                return true;
            }

            clip = null;
            volume = 0f;
            pitch = 1f;
            return false;
        }

        private void BuildLookupIfNeeded()
        {
            if (_lookup != null)
            {
                return;
            }

            _lookup = new Dictionary<SfxId, SfxEntry>();

            foreach (SfxEntry entry in _entries)
            {
                _lookup[entry.Id] = entry;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _lookup = null;

            foreach (SfxId id in (SfxId[])Enum.GetValues(typeof(SfxId)))
            {
                bool found = false;

                foreach (SfxEntry entry in _entries)
                {
                    if (entry.Id == id && entry.Clip != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Debug.LogWarning($"SfxConfig: no clip assigned for {id}", this);
                }
            }
        }
#endif
    }
}
