var input = File.ReadAllText("./data.txt");

var almenac = new Almenac(input);

var result1 = almenac.GetMinInNormalSeeds();
Console.WriteLine($"Result 1: {result1}");

var result2 = almenac.GetMinInSeedRanges();
Console.WriteLine($"Result 2: {result2}");