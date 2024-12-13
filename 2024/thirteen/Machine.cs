using System.Text.RegularExpressions;

public record Machine(Coordinate A, Coordinate B, Coordinate Price)
{
    public static Machine Parse(string[] lines)
    {
        return new Machine(ParseButton(lines[0]), ParseButton(lines[1]), ParsePrize(lines[2]));
    }

    private static Coordinate ParseButton(string line)
    {
        var match = Regex.Match(line, @"X\+(\d+), Y\+(\d+)");
        return new Coordinate(long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
    }

    private static Coordinate ParsePrize(string line)
    {
        var match = Regex.Match(line, @"X=(\d+), Y=(\d+)");
        return new Coordinate(long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
    }
    
    // solve system of equations with cramers rule
    public long? GetNumTokens(Coordinate target)
    {

        long numeratorB = target.X * B.Y - target.Y * B.X;
        long numeratorA = -target.X * A.Y + target.Y * A.X;

        long det = A.X * B.Y - A.Y * B.X;
        if (numeratorA % det != 0 || numeratorB % det != 0)
        {
            return null;
        }

        long a = numeratorB / det;
        long b = numeratorA / det;

        return 3 * a + b;
    }
}
