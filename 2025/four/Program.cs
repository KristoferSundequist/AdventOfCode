var lines = File.ReadAllLines("input.txt");
var diagram = new Diagram(lines);

Console.WriteLine($"Part1: {diagram.GetAccessibleRollsOfPaper().Count()}");

var removedRollsOfPaper = diagram.RemoveAllPaperThatYouCan();
Console.WriteLine($"Part2: {removedRollsOfPaper}");