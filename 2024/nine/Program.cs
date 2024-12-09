var inputfile = "data.txt";
var disc = new Disk(inputfile);
disc.Defrag();
var checksum = disc.CalculateChecksum();
Console.WriteLine($"Part 1: {checksum}");

var disc2 = new Disk(inputfile);
disc2.Defrag2();
var checksum2 = disc2.CalculateChecksum();
Console.WriteLine($"Part 2: {checksum2}");
