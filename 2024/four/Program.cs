var lines = File.ReadAllLines("data.txt").Select(x => x.ToLower()).ToArray();

Part1();
Part2();

void Part1()
{
    var totalCount = 0;
    for (var y = 0; y < lines.Length; y++)
    {
        for (var x = 0; x < lines[y].Length; x++)
        {
            totalCount += CountForCoord(lines, y, x);
        }
    }
    Console.WriteLine(totalCount);
}

int CountForCoord(string[] input, int y, int x)
{
    var totalCount = 0;

    // vertical
    if (y + 3 < lines.Length)
    {
        {
            if (
                (lines[y][x] == 'x' && lines[y + 1][x] == 'm' && lines[y + 2][x] == 'a' && lines[y + 3][x] == 's') ||
                (lines[y][x] == 's' && lines[y + 1][x] == 'a' && lines[y + 2][x] == 'm' && lines[y + 3][x] == 'x')
            )
            {
                totalCount++;
            }
        }
    }

    // horizontal
    if (x + 3 < lines[y].Length)
    {
        if (
            (lines[y][x] == 'x' && lines[y][x + 1] == 'm' && lines[y][x + 2] == 'a' && lines[y][x + 3] == 's') ||
            (lines[y][x] == 's' && lines[y][x + 1] == 'a' && lines[y][x + 2] == 'm' && lines[y][x + 3] == 'x')
        )
        {
            totalCount++;
        }
    }

    // diagonal down-right
    if (y + 3 < lines.Length && x + 3 < lines[y].Length)
    {
        if (
            (lines[y][x] == 'x' && lines[y + 1][x + 1] == 'm' && lines[y + 2][x + 2] == 'a' && lines[y + 3][x + 3] == 's') ||
            (lines[y][x] == 's' && lines[y + 1][x + 1] == 'a' && lines[y + 2][x + 2] == 'm' && lines[y + 3][x + 3] == 'x')
        )
        {
            totalCount++;
        }
    }

    // diagonal down-left
    if (y + 3 < lines.Length && x - 3 >= 0)
    {
        if (
            (lines[y][x] == 'x' && lines[y + 1][x - 1] == 'm' && lines[y + 2][x - 2] == 'a' && lines[y + 3][x - 3] == 's') ||
            (lines[y][x] == 's' && lines[y + 1][x - 1] == 'a' && lines[y + 2][x - 2] == 'm' && lines[y + 3][x - 3] == 'x')
        )
        {
            totalCount++;
        }
    }

    return totalCount;
}

void Part2()
{
    var totalCount = 0;
    for (var y = 0; y < lines.Length - 2; y++)
    {
        for (var x = 0; x < lines[y].Length - 2; x++)
        {
            if (
                (lines[y][x] == 'm' && lines[y + 1][x + 1] == 'a' && lines[y + 2][x + 2] == 's') ||
                (lines[y][x] == 's' && lines[y + 1][x + 1] == 'a' && lines[y + 2][x + 2] == 'm')
            )
            {
                if (
                    (lines[y][x + 2] == 's' && lines[y + 1][x + 1] == 'a' && lines[y + 2][x] == 'm') ||
                    (lines[y][x + 2] == 'm' && lines[y + 1][x + 1] == 'a' && lines[y + 2][x] == 's')
                )
                {
                    totalCount++;
                }
            }

        }
    }
    Console.WriteLine(totalCount);
}