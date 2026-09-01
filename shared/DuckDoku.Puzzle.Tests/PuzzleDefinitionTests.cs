namespace DuckDoku.Puzzle.Tests;

public class PuzzleDefinitionTests
{
    [Test]
    public void Constructor_ValidArguments_ExposesSizeSeedAndDifficulty()
    {
        PuzzleDefinition definition = TestPuzzles.CreateDefinition();

        Assert.Multiple(() =>
        {
            Assert.That(definition.Difficulty, Is.EqualTo(TestPuzzles.Difficulty));
            Assert.That(definition.Size, Is.EqualTo(TestPuzzles.Size));
            Assert.That(definition.Seed, Is.EqualTo(TestPuzzles.Seed));
        });
    }

    [Test]
    public void Constructor_ValidArguments_ExposesSolution()
    {
        PuzzleDefinition definition = TestPuzzles.CreateDefinition();
        
        Assert.That(definition.Solution, Has.Count.EqualTo(TestPuzzles.Size));
        Assert.Multiple(() =>                                                        
        {                                                                            
            for (int row = 0; row < TestPuzzles.Size; row++)                                     
            {                                                                        
                Cell cell = definition.Solution[row];                                
                                                                                       
                Assert.That(cell.Row, Is.EqualTo(row));      
                Assert.That(cell.Column, Is.EqualTo(TestPuzzles.SolutionColumns[row]));                                                                      
            }                                                                        
        }); 
    }
    
    [Test]                                                                           
    public void Constructor_Mutation_DoesNotAffectDefinition()            
    {                                                                                
        int[] regions = TestPuzzles.CreateRegions();                                             
        Cell[] solution = TestPuzzles.CreateSolution();                                          
                                                                                       
        PuzzleDefinition definition = new PuzzleDefinition(TestPuzzles.Size, TestPuzzles.Seed, TestPuzzles.Difficulty, regions, solution);                               
                                                                                       
        regions[0] = TestPuzzles.Size - 1;                                                       
        solution[0] = new Cell(TestPuzzles.Size - 1, TestPuzzles.Size - 1);                                  
                                                                                       
        Assert.Multiple(() =>                                                        
        {                                                                            
            Assert.That(definition.RegionAt(0, 0), Is.EqualTo(0));                   
            Assert.That(definition.Solution[0], Is.EqualTo(
                new Cell(0, TestPuzzles.SolutionColumns[0])));                                                               
        });                                                                          
    }         
}