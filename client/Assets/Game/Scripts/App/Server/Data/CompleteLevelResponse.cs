using System;

namespace DuckDoku.App
{
    [Serializable]
    public class CompleteLevelResponse
    {
        public long duration;
        public int stars;
        public int balance;
        public int coinsEarned;
    }
}
