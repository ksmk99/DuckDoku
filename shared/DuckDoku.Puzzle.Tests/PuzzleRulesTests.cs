namespace DuckDoku.Puzzle.Tests;

public class PuzzleRulesTests
{
    [Test]
    public void IsSolved_ValidSolution_ReturnsTrue()
    {
        PuzzleDefinition definition = TestPuzzles.CreateDefinition();
        Assert.That(PuzzleRules.IsSolved(definition, definition.Solution), Is.True);
    }

    [Test]
    public void IsSolved_IncompletePlacement_ReturnsFalse()
    {
        PuzzleDefinition definition = TestPuzzles.CreateDefinition();

        Cell[] placements = definition.Solution.Take(TestPuzzles.Size - 1).ToArray();

        Assert.That(PuzzleRules.IsSolved(definition, placements), Is.False);
    }

    [Test]
    public void FindViolations_ValidSolution_ReturnsEmpty()
    {
        PuzzleDefinition definition = TestPuzzles.CreateDefinition();

        Assert.That(PuzzleRules.FindViolations(definition, definition.Solution), Is.Empty);
    }

    [Test]
    public void TryFindConflict_ConflictingSolution_ReturnsViolation()
    {
        PuzzleDefinition definition = TestPuzzles.CreateDefinition();

        Cell[] occupied = TestPuzzles.CreateSolution().Take(3).ToArray();
        var candidate = new Cell(occupied[0].Row, TestPuzzles.Size - 1);
        RuleViolation violation = new RuleViolation();

        Assert.That(PuzzleRules.TryFindConflict(definition, occupied, candidate, out violation), Is.True);
        Assert.That(violation, Is.EqualTo(new RuleViolation(RuleViolationType.Row, occupied[0], candidate)));
    }

    [Test]
    public void TryFindConflict_FreeCandidate_ReturnsFalse()
    {
        PuzzleDefinition definition = TestPuzzles.CreateDefinition();

        Cell[] occupied = { new Cell(0, 0) };
        Cell candidate = new Cell(3, 3);

        Assert.That(
            PuzzleRules.TryFindConflict(definition, occupied, candidate, out RuleViolation _),
            Is.False);
    }
}