using System;
using System.Collections.Generic;

namespace DuckDoku.Puzzle
{
    public static class PuzzleRules
    {
        public static bool IsSolved(PuzzleDefinition definition, IReadOnlyList<Cell> placements)
        {
            if (placements.Count != definition.Size)
            {
                return false;
            }

            return FindViolations(definition, placements).Count == 0;
        }

        public static IReadOnlyList<RuleViolation> FindViolations(
            PuzzleDefinition definition,
            IReadOnlyList<Cell> occupied)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (occupied == null)
            {
                throw new ArgumentNullException(nameof(occupied));
            }

            var violations = new List<RuleViolation>();
            for (int i = 0; i < occupied.Count; i++)
            {
                Cell first = occupied[i];
                int firstRegion = definition.RegionAt(first);

                for (int j = i + 1; j < occupied.Count; j++)
                {
                    Cell second = occupied[j];

                    if (first.Row == second.Row)
                    {
                        violations.Add(
                            new RuleViolation(RuleViolationType.Row, first, second));
                    }

                    if (first.Column == second.Column)
                    {
                        violations.Add(
                            new RuleViolation(RuleViolationType.Column, first, second));
                    }

                    if (firstRegion == definition.RegionAt(second))
                    {
                        violations.Add(
                            new RuleViolation(RuleViolationType.Region, first, second));
                    }

                    if (AreAdjacent(first, second))
                    {
                        violations.Add(
                            new RuleViolation(RuleViolationType.Adjacency, first, second));
                    }
                }
            }
            
            return violations;
        }

        public static bool TryFindConflict(
            PuzzleDefinition definition,
            IReadOnlyList<Cell> occupied,
            Cell candidate,
            out RuleViolation violation)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (occupied == null)
            {
                throw new ArgumentNullException(nameof(occupied));
            }

            int candidateRegion = definition.RegionAt(candidate);
            for (int i = 0; i < occupied.Count; i++)
            {
                Cell placed = occupied[i];
                if (placed.Row == candidate.Row)
                {
                    violation = new RuleViolation(RuleViolationType.Row, placed, candidate);
                    return true;
                }

                if (placed.Column == candidate.Column)
                {
                    violation = new RuleViolation(RuleViolationType.Column, placed, candidate);
                    return true;
                }

                if (definition.RegionAt(placed) == candidateRegion)
                {
                    violation = new RuleViolation(RuleViolationType.Region, placed, candidate);
                    return true;
                }

                if (AreAdjacent(placed, candidate))
                {
                    violation = new RuleViolation(RuleViolationType.Adjacency, placed, candidate);
                    return true;
                }
            }

            violation = default;
            return false;
        }

        private static bool AreAdjacent(Cell first, Cell second)
        {
            if (first == second)
            {
                return false;
            }

            return Math.Abs(first.Row - second.Row) <= 1
                   && Math.Abs(first.Column - second.Column) <= 1;
        }
    }
}