var lines = File.ReadAllLines("input.txt");

var machines = lines.Chunk(4).Select(Machine.Parse).ToList();
var totalTokens = machines.Sum(m => m.GetNumTokens(m.Price) ?? 0);
Console.WriteLine($"Part 1: {totalTokens}");

var totalTokens2 = machines.Sum(m => m.GetNumTokens(m.Price + 10000000000000) ?? 0);
Console.WriteLine($"Part 2: {totalTokens2}");