using DuckDoku.Domain;

namespace DuckDoku.Presentation
{
    public class LevelModel
    {
        public LevelSummary LevelSummary { get; }

        public LevelModel(LevelSummary levelSummary)
        {
            LevelSummary = levelSummary;
        }
    }
}