using System.Text.RegularExpressions;

public record Robot(DiscreteVector2 Position, DiscreteVector2 Velocity)
{
    public static Robot Parse(string input)
    {
        var match = Regex.Match(input, @"p=(?<pX>-?\d+),(?<pY>-?\d+) v=(?<vX>-?\d+),(?<vY>-?\d+)");
        if (!match.Success)
        {
            throw new ArgumentException("Invalid input", input);
        }

        var pX = long.Parse(match.Groups["pX"].Value);
        var pY = long.Parse(match.Groups["pY"].Value);
        var vX = long.Parse(match.Groups["vX"].Value);
        var vY = long.Parse(match.Groups["vY"].Value);

        return new Robot(new DiscreteVector2(pX, pY), new DiscreteVector2(vX, vY));
    }

    public Robot Tick(DiscreteVector2 bottomRightCorner) => new((Position + Velocity).Mod(bottomRightCorner), Velocity);
    public int? GetQuadrant(DiscreteVector2 bottomRightCorner)
    {
        var horizontalMiddleLine = bottomRightCorner.Y / 2;
        var verticalMiddleLine = bottomRightCorner.X / 2;

        if (Position.X < verticalMiddleLine && Position.Y < horizontalMiddleLine)
        {
            return 1;
        }

        if (Position.X > verticalMiddleLine && Position.Y < horizontalMiddleLine)
        {
            return 2;
        }

        if (Position.X < verticalMiddleLine && Position.Y > horizontalMiddleLine)
        {
            return 3;
        }

        if (Position.X > verticalMiddleLine && Position.Y > horizontalMiddleLine)
        {
            return 4;
        }

        return null;
    }
}

public record DiscreteVector2(long X, long Y)
{
    public static DiscreteVector2 operator +(DiscreteVector2 a, DiscreteVector2 b) => new(a.X + b.X, a.Y + b.Y);
    public DiscreteVector2 Mod(DiscreteVector2 other)
    {
        return new(Mod(X, other.X), Mod(Y, other.Y));
    }

    private static long Mod(long x, long m)
    {
        if (x < 0)
        {
            return m + x;
        }

        return x % m;
    }
}