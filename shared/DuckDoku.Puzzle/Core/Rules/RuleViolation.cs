namespace DuckDoku.Puzzle
{
    public readonly struct RuleViolation
    {
        public RuleViolationType Type { get; }
        
        public Cell CellOne { get; }
        public Cell CellTwo { get; }

        public RuleViolation(RuleViolationType type, Cell cellOne, Cell cellTwo)
        {
            Type = type;
            
            CellOne = cellOne.Row < cellTwo.Row ? cellOne : cellTwo;
            CellTwo = cellOne.Row < cellTwo.Row ? cellTwo : cellOne;
        }

        public override string ToString()
        {
            return  $"{Type}: {CellOne} -> {CellTwo}";
        }
    }
}