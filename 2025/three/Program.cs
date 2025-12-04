using System.Text;

var banks = File.ReadAllLines("input.txt");

var part1 = banks.Sum(bank => GetBiggestJoltage2(bank, 2));
Console.WriteLine($"Part1: {part1}");

var part2 = banks.Sum(bank => GetBiggestJoltage2(bank, 12));
Console.WriteLine($"Part2: {part2}");

long GetBiggestJoltage2(string bank, int numBatteriesToTurnOn)
{
    var joltage = new StringBuilder();
    var currentIndex = 0;
    for (var i = 0; i < numBatteriesToTurnOn; i++)
    {
        var (nextIndex, value) = GetNextBiggest(bank, currentIndex, numBatteriesToTurnOn - i);
        currentIndex = nextIndex + 1;
        joltage.Append(value);
    }
    return long.Parse(joltage.ToString());
}

(int index, char value) GetNextBiggest(string bank, int startIndex, int remaining)
{
    var maxFirstValue = '0';
    var maxFirstIndex = -1;

    for (var i = startIndex; i < bank.Length - remaining + 1; i++)
    {
        if (bank[i] > maxFirstValue)
        {
            maxFirstValue = bank[i];
            maxFirstIndex = i;
        }
    }

    return (maxFirstIndex, maxFirstValue);
}