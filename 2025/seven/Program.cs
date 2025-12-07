var lines = File.ReadAllLines("input.txt");
var diagram = new ManifoldDiagram(lines);

var result = diagram.GetNumSplits();
Console.WriteLine($"Part1: {result.numSplits}");
Console.WriteLine($"Part2: {result.numTimeLines}");