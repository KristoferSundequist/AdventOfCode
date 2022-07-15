
var lines = System.IO.File.ReadAllLines("data.txt");

Console.WriteLine(Part1(lines));
Console.WriteLine(Part2(lines));

long Part2(string[] lines)
{
    var oxy = GetRating(lines, (tempLines, bitToConsider) =>
    {
        var (mostCommon, _) = GetCommonness(tempLines, bitToConsider);
        return tempLines.Where(line => CharToInt(line[bitToConsider]) == mostCommon).ToArray();
    });
    var co2 = GetRating(lines, (tempLines, bitToConsider) =>
    {
        var (_, leastCommon) = GetCommonness(tempLines, bitToConsider);
        return tempLines.Where(line => CharToInt(line[bitToConsider]) == leastCommon).ToArray();
    });
    return oxy * co2;
}

long GetRating(string[] lines, Func<string[], int, string[]> filter)
{
    int bitToConsider = 0;
    while (true)
    {
        lines = filter(lines, bitToConsider);
        if (lines.Length == 1)
        {
            return BinaryToDecimal(lines[0].Select(CharToInt).ToList());
        }
        bitToConsider += 1;
    }
};

int CharToInt(char c) => Int32.Parse(c.ToString());

(int, int) GetCommonness(string[] lines, int bitToConsider)
{
    var lineLength = lines[0].Length;
    var ones = 0;
    var numLines = lines.Length;
    for (var i = 0; i < numLines; i++)
    {
        ones += CharToInt(lines[i][bitToConsider]);
    }
    var mostCommon = ones >= numLines - ones ? 1 : 0;
    var leastCommon = ones >= numLines - ones ? 0 : 1;
    return (mostCommon, leastCommon);
}

long Part1(string[] lines)
{
    var lineLength = lines[0].Length;
    var ones = Enumerable.Range(0, lineLength).Select(_ => 0).ToList();
    var numLines = lines.Length;
    for (var i = 0; i < numLines; i++)
    {
        for (var j = 0; j < lineLength; j++)
        {
            ones[j] += CharToInt(lines[i][j]);
        }
    }
    var mostCommon = ones.Select(count => count > numLines - count ? 1 : 0).ToList();
    var leastCommon = mostCommon.Select(n => n == 1 ? 0 : 1).ToList();
    var mostCommonInDecimal = BinaryToDecimal(mostCommon);
    var leastCommonInDecimal = BinaryToDecimal(leastCommon);
    return mostCommonInDecimal * leastCommonInDecimal;
}



long BinaryToDecimal(List<int> binary)
{
    long dec = 0;
    for (var i = 0; i < binary.Count; i++)
    {
        dec += binary[i] * (long)Math.Pow(2, binary.Count - i - 1);
    }
    return dec;
}