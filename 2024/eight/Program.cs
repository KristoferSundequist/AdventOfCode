var lines = File.ReadAllLines("data.txt").Select(l => l.ToCharArray()).ToArray();

var grid = new Grid(lines);
Console.WriteLine($"Part 1: {grid.GetAllAntiNodes([1]).Count()}");
Console.WriteLine($"Part 1: {grid.GetAllAntiNodes(Enumerable.Range(0, 50)).Count()}");