var input = File.ReadAllLines("./data.txt");
var image = new Image(input);
//image.Draw();
var result1 = image.GetPairDistanceSum(1);
Console.WriteLine($"Result 1: {result1}");

var result2 = image.GetPairDistanceSum(999999);
Console.WriteLine($"Result 2: {result2}");