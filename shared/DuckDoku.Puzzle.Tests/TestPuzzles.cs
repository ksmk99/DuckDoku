namespace DuckDoku.Puzzle.Tests;

public static class TestPuzzles
{
    public const int Size = 6;
    public const long Seed = 42;
    public const PuzzleDifficulty Difficulty = PuzzleDifficulty.Easy;
    public static readonly int[] SolutionColumns = { 1, 3, 5, 0, 2, 4 };

    public static PuzzleDefinition CreateDefinition()
    {
        var regions = CreateRegions();
        var solution = CreateSolution();
        
        return new PuzzleDefinition(Size, Seed, Difficulty, regions, solution); 
    }
    
    public static int[] CreateRegions()
    {
        int[] regions = new int[Size * Size];

        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                regions[row * Size + column] = row;
            }
        }
        
        return regions;
    }

    public static Cell[] CreateSolution()
    {
        Cell[] solution = new Cell[Size];
        for (int row = 0; row < Size; row++)
        {
            solution[row] = new Cell(row, SolutionColumns[row]);
        }
        
        return solution;
    }
}