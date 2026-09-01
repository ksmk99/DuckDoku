using System.Collections.Generic;

namespace DuckDoku.Puzzle
{
    public readonly struct SolveResult
    {
        public SolutionCount Count { get; }
        public IReadOnlyList<Cell> Solution { get; }

        public SolveResult(SolutionCount count, IReadOnlyList<Cell> solution)
        {
            Count = count;
            Solution = solution;
        }
    }
}