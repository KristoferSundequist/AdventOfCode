var input = File.ReadAllLines("input.txt").Select(line => line.ToCharArray()).ToArray();

var map = new Map(input);
var result = map.GetAllTrailHeadScores(false);
Console.WriteLine($"Part 1: {result}");

result = map.GetAllTrailHeadScores(true);
Console.WriteLine($"Part 2: {result}");