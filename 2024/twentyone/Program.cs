Dictionary<(string, int), long> expandcache = new();
var codes = File.ReadAllLines("codes.txt");
var numericalKeypad = Keypad.NumericKeypad();
var directionalKeypad = Keypad.DirectionalKeypad();

long totalScore = codes.Select(str => GetScore(str, 25)).Sum();
Console.WriteLine(totalScore);

long GetScore(string str, int numDirectional)
{
    var baseProblem = numericalKeypad.GetWholePaths(str);
    var length = baseProblem.Sum(v => Expand(v, numDirectional));
    return length * int.Parse(str[..^1]);
}

long Expand(string path, int numDirectional)
{
    if (expandcache.ContainsKey((path, numDirectional)))
    {
        return expandcache[(path, numDirectional)];
    }
    if (numDirectional == 0)
    {
        return path.Length;
    }

    var paths = directionalKeypad.GetWholePaths(path);
    var result = paths.Sum(p => Expand(p, numDirectional - 1));
    expandcache[(path, numDirectional)] = result;
    return result;
}