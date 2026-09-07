using System;

namespace DuckDoku.App
{
    [Serializable]
    public class StartLevelResponse
    {
        public string sessionId;
        public int energy;
        public int energyMax;
        public long energyRefillMs;
    }
}
