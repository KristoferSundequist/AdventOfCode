var map = new Map(File.ReadAllLines("map.txt"));
var moves = File.ReadAllText("moves.txt").Replace(Environment.NewLine, "");

map.MakeMoves(moves);
var gpsSum = map.GetGPSSum();
Console.WriteLine($"Part 1: {gpsSum}");


var wideMap = new WideMap(File.ReadAllLines("map.txt"));
wideMap.MakeMoves(moves);
var wideGpsSum = wideMap.GetGPSSum();
Console.WriteLine($"Part 2: {wideGpsSum}");


