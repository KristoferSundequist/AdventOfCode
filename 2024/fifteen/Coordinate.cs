public record Coordinate(long Y, long X)
{
    public static Coordinate operator +(Coordinate a, Coordinate b) => new(a.Y + b.Y, a.X + b.X);

    public long GetGpsCoordnate() => Y * 100 + X;

    public bool IsHorizontal() => Y == 0 && X != 0;
    public bool IsVertical() => Y != 0 && X == 0;
    public Coordinate MoveUp() => new(Y - 1, X);
    public Coordinate MoveDown() => new(Y + 1, X);
    public Coordinate MoveLeft() => new(Y, X - 1);
    public Coordinate MoveRight() => new(Y, X + 1);
    
    public static Coordinate Up() => new(-1, 0);
    public static Coordinate Down() => new(1, 0);
    public static Coordinate Left() => new(0, -1);
    public static Coordinate Right() => new(0, 1);
}