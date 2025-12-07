var lines = File.ReadAllLines("input.txt");
var diagram = new ManifoldDiagram(lines);

Console.WriteLine($"Part1: {diagram.GetNumSplits()}");