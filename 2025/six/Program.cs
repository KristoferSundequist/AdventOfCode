var lines = File.ReadAllLines("input.txt");

Console.WriteLine($"Part 1: {Part1(lines)}");

Console.WriteLine($"Part 2: {Part2(lines)}");

long Part1(string[] lines)
{
    var cleanLines = lines.Select(line => line.Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToArray()).ToArray();
    var numberLines = cleanLines[..^1].Select(line => line.Select(x => long.Parse(x)).ToArray()).ToArray();
    var operatorLine = cleanLines[^1];

    long sum = 0;
    for (int x = 0; x < operatorLine.Length; x++)
    {
        long start = operatorLine[x] == "*" ? 1 : 0;
        for (var y = 0; y < numberLines.Length; y++)
        {
            if (operatorLine[x] == "*")
            {
                start *= numberLines[y][x];
            }
            else
            {
                start += numberLines[y][x];
            }
        }
        sum += start;
    }
    return sum;
}

long Part2(string[] lines)
{
    var charLines = lines.Select(line => line.ToCharArray()).ToArray();
    var numberLines = charLines[..^1];
    var operatorLine = charLines[^1];

    char currentOperator = operatorLine[0];
    long sum = 0;
    long currentAggregation = 0;
    for (int x = 0; x < operatorLine.Length; x++)
    {
        // new number group starts here
        if (operatorLine[x] == '*' || operatorLine[x] == '+')
        {
            sum += currentAggregation;
            currentOperator = operatorLine[x];
            currentAggregation = currentOperator == '*' ? 1 : 0;
        }
        var columnStr = string.Join("", numberLines.Select(line => line[x])).Trim();

        if (long.TryParse(columnStr, out var column))
        {
            if (currentOperator == '*')
            {
                currentAggregation *= column;
            }
            else
            {
                currentAggregation += column;
            }
        }
    }
    sum += currentAggregation;
    return sum;
}