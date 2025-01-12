public record Coordinate(int Y, long X)
{
    public Coordinate Move(Direction direction) => direction switch
    {
        Direction.North => new Coordinate(Y - 1, X),
        Direction.East => new Coordinate(Y, X + 1),
        Direction.South => new Coordinate(Y + 1, X),
        Direction.West => new Coordinate(Y, X - 1),
        _ => throw new InvalidOperationException()
    };
    public static Coordinate Zero => new Coordinate(0, 0);
}