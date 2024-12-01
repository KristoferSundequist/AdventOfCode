var input = File.ReadAllLines("data.txt").Select(line => line.Split("   "));

var list1 = input.Select(line => long.Parse(line[0])).Order();
var list2 = input.Select(line => long.Parse(line[1])).Order();

var result1 = list1.Zip(list2, (a, b) => Math.Abs(a - b)).Sum();
Console.WriteLine($"Result1: {result1}");

var list2tally = list2.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
var result2 = list1.Select(x =>
{
    if (list2tally.TryGetValue(x, out var count))
    {
        return x * count;
    }
    return 0;
}).Sum();
Console.WriteLine($"Result2: {result2}");