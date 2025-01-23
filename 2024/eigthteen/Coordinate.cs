public record struct Coordinate(long X, long Y)
{
    public static Coordinate FromString(string input)
    {
        var parts = input.Split(',');
        return new Coordinate(long.Parse(parts[0]), long.Parse(parts[1]));
    }

    public Coordinate Up() => new Coordinate(X, Y - 1);
    public Coordinate Down() => new Coordinate(X, Y + 1);
    public Coordinate Left() => new Coordinate(X - 1, Y);
    public Coordinate Right() => new Coordinate(X + 1, Y);
}