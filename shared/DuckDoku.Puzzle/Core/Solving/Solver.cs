using System;

namespace DuckDoku.Puzzle
{
    public static class Solver
    {
        private const int NoPreviousColumn = -2;
        
        public static SolveResult Solve(PuzzleDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            int size = definition.Size;
            
            int[] buffer = new int[size];
            Cell[]? firstSolution = null;
            int found = 0;

            Search(definition, 0, 0, 0, NoPreviousColumn, buffer, ref found, ref firstSolution);

            if (found == 0)
            {
                return new SolveResult(SolutionCount.None, null);
            }

            if (found >= 2)
            {
                return new SolveResult(SolutionCount.Multiple, null);
            }

            return new SolveResult(SolutionCount.Unique, firstSolution!);
        }

        private static void Search(
            PuzzleDefinition definition,
            int row,
            int columnsMask,
            int regionsMask,
            int previousColumn,
            int[] buffer,
            ref int found,
            ref Cell[]? firstSolution)
        {
            int size = definition.Size;

            if (row == size)                                                                                                 
            {                                                                                                                
                found++;                                                                                                     
                                                                                                                   
                if (found == 1)                                                                                              
                {                                                                                                            
                    firstSolution = new Cell[size];                                                                          
                                                                                                                   
                    for (int i = 0; i < size; i++)                                                                           
                    {                                                                                                        
                        firstSolution[i] = new Cell(i, buffer[i]);                                                           
                    }                                                                                                        
                }                                                                                                            
                                                                                                                   
                return;                                                                                                      
            }

            for (int column = 0; column < size; column++)
            {
                if ((columnsMask & (1 << column)) != 0)
                {
                    continue;
                }

                int region = definition.RegionAt(row, column);

                if ((regionsMask & (1 << region)) != 0)
                {
                    continue;
                }
                
                int distance = column - previousColumn;
                if (distance >= -1 && distance <= 1)
                {
                    continue;
                }

                buffer[row] = column;
                Search(
                    definition,
                    row + 1,
                    columnsMask | (1 << column),
                    regionsMask | (1 << region),
                    column,
                    buffer,
                    ref found,
                    ref firstSolution);
                
                if (found >= 2)
                {
                    return;
                }
            }
        }
    }
}