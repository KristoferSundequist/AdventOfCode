var input = File.ReadAllLines("input.txt").Select(line => line.ToCharArray()).ToArray();

var map = new Map(input);
Console.WriteLine($"Part1: {map.GetTotalFencePrice1()}");
Console.WriteLine($"Part2: {map.GetTotalFencePrice2()}");