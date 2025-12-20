var lines = System.IO.File.ReadAllLines("input.txt");
var machines = lines.Select(line => new Machine(line)).ToList();

var result1 = machines.Sum(m => m.Part1());
Console.WriteLine($"Part1: {result1}");

var result2 = machines.Sum(m => m.Part2());
Console.WriteLine($"Part2: {result2}");