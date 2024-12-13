public record Coordinate(long Y, long X)
{
    public static Coordinate operator +(Coordinate a, Coordinate b) => new Coordinate(a.Y + b.Y, a.X + b.X);
    public static Coordinate operator -(Coordinate a, Coordinate b) => new Coordinate(a.Y - b.Y, a.X - b.X);
    public static Coordinate operator *(Coordinate a, long b) => new Coordinate(a.Y * b, a.X * b);
    public static Coordinate operator *(long a, Coordinate b) => new Coordinate(b.Y * a, b.X * a);
    public static Coordinate operator /(Coordinate a, long b) => new Coordinate(a.Y / b, a.X / b);
    public static Coordinate operator +(Coordinate a, long b) => new Coordinate(a.Y + b, a.X + b);
    public static Coordinate operator -(Coordinate a, long b) => new Coordinate(a.Y - b, a.X - b);
}