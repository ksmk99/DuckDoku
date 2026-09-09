#if UNITY_EDITOR
using System;

namespace DuckDoku.App
{
    [Serializable]
    public class DevEnergyResponse
    {
        public int energy;
        public int energyMax;
        public long energyRefillMs;
    }
}
#endif
