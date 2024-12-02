var input = File.ReadAllLines("data.txt").Select(line => line.Split(' ').Select(level => int.Parse(level)).ToArray()).ToArray();

var result1 = input.Where(IsSafe).Count();
Console.WriteLine(result1);

var result2 = input.Where(IsSafe2).Count();
Console.WriteLine(result2);

bool IsSafe(int[] levels)
{
    var isIncreasing = levels[1] - levels[0] > 0;
    for (var j = 1; j < levels.Length; j++)
    {
        var diff = levels[j] - levels[j - 1];
        if (diff == 0)
        {
            return false;
        }
        if (Math.Abs(diff) > 3)
        {
            return false;
        }
        if (isIncreasing && diff < 0)
        {
            return false;
        }
        if (!isIncreasing && diff > 0)
        {
            return false;
        }
    }
    return true;
}

bool IsSafe2(int[] levels)
{
    for (var i = 0; i < levels.Length; i++)
    {
        var levelsWithoutLevel = levels[..i].Concat(levels[(i + 1)..]);
        if (IsSafe(levelsWithoutLevel.ToArray()))
        {
            return true;
        }
    }
    return false;
}
