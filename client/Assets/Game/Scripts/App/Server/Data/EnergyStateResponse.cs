using System;

namespace DuckDoku.App
{
    [Serializable]
    public class EnergyStateResponse
    {
        public int energy;
        public int energyMax;
        public long energyRefillMs;
    }
}
