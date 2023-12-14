using System.Text;

var patterns = File.ReadAllText("data.txt").Split($"{Environment.NewLine}{Environment.NewLine}").Select(pattern => Pattern.FromString(pattern)).ToArray();

var result1 = patterns.Sum(p => p.GetReflectionLine().GetScore());
Console.WriteLine($"Result 1: {result1}");

var result2 = patterns.Sum(p => p.GetUnsmudgedReflectionLine().GetScore());
Console.WriteLine($"Result 2: {result2}");

public class Pattern
{
    public required string[] Lines { get; init; }

    public static Pattern FromString(string input)
    {
        return new Pattern { Lines = input.Split(Environment.NewLine) };
    }

    public static Pattern FromLines(string[] inputLines)
    {
        return new Pattern { Lines = inputLines };
    }

    public Result GetUnsmudgedReflectionLine()
    {
        var originalScore = GetReflectionLine();
        for (var y = 0; y < Lines.Length; y++)
        {
            for (var x = 0; x < Lines[0].Length; x++)
            {
                var result = Pattern.FromLines(Flipped(Lines, y, x)).GetReflectionLine(originalScore);
                if (result.success)
                {
                    return result;
                }
            }
        }
        throw new Exception("Couldnt find a line");
    }

    private static string[] Flipped(string[] original, int y, int x)
    {
        return original.Select((line, line_number) =>
        {
            if (line_number == y)
            {
                var strBuilder = new StringBuilder(original[line_number]);
                if (original[y][x] == '#')
                {
                    strBuilder[x] = '.';
                }
                else
                {
                    strBuilder[x] = '#';
                }
                return strBuilder.ToString();
            }
            return line;
        }).ToArray();
    }

    public Result GetReflectionLine(Result? bannedResult = null)
    {
        var verticalResult = FindVerticalLineIndex(bannedResult);
        if (verticalResult.success)
        {
            return new Result(true, verticalResult.value, false);
        }
        var horizontalResult = FindHorizontalLineIndex(bannedResult);
        if (horizontalResult.success)
        {
            return new Result(true, horizontalResult.value, true);
        }
        return new Result(false, -1, true);
    }

    public Result FindVerticalLineIndex(Result? bannedResult = null)
    {
        for (var x = 0; x < Lines[0].Length - 1; x++)
        {
            if (ColsEqual(x, x + 1) && CheckVerticalLineIndex(x))
            {
                var result = new Result(true, x + 1, false);
                if (result != bannedResult)
                {
                    return result;
                }
            }
        }
        return new Result(false, -1, false);
    }

    public Result FindHorizontalLineIndex(Result? bannedResult = null)
    {
        for (var y = 0; y < Lines.Length - 1; y++)
        {
            if (RowsEqual(y, y + 1) && CheckHorizontalLineIndex(y))
            {
                var result = new Result(true, y + 1, true);
                if (result != bannedResult)
                {
                    return result;
                }
            }
        }
        return new Result(false, -1, true);
    }

    private bool CheckHorizontalLineIndex(int index)
    {
        for (var y = 1; y < Lines.Length; y++)
        {
            var index1 = index - y;
            var index2 = index + y + 1;
            if (index1 < 0) return true;
            if (index2 >= Lines.Length) return true;

            if (!RowsEqual(index1, index2))
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckVerticalLineIndex(int index)
    {
        for (var x = 1; x < Lines[0].Length; x++)
        {
            var index1 = index - x;
            var index2 = index + x + 1;
            if (index1 < 0) return true;
            if (index2 >= Lines[0].Length) return true;

            if (!ColsEqual(index1, index2))
            {
                return false;
            }
        }
        return true;
    }

    private bool ColsEqual(int col1, int col2)
    {
        for (var y = 0; y < Lines.Length; y++)
        {
            if (Lines[y][col1] != Lines[y][col2])
            {
                return false;
            }
        }
        return true;
    }

    private bool RowsEqual(int row1, int row2)
    {
        for (var x = 0; x < Lines[0].Length; x++)
        {
            if (Lines[row1][x] != Lines[row2][x])
            {
                return false;
            }
        }
        return true;
    }
}

public record Result(bool success, int value, bool horizontal)
{
    public int GetScore() => value * (horizontal ? 100 : 1);
}