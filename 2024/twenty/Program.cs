var lines = File.ReadAllLines("map.txt").Select(l => l.ToCharArray().Select(c => c.ToString()).ToArray()).ToArray();
var map = new Map(lines);

var part1 = map.NumCheatsPerSaved(2);
var part1result = part1.Where(kvp => kvp.Key >= 100).Sum(kvp => kvp.Value);
Console.WriteLine($"Part 1: {part1result}");

var part2 = map.NumCheatsPerSaved(20);
var part2result = part2.Where(kvp => kvp.Key >= 100).Sum(kvp => kvp.Value);
Console.WriteLine($"Part 2: {part2result}");