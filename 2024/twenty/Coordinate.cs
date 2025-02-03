public record struct Coordinate(int Y, int X)
{
    public static Coordinate operator +(Coordinate a, Coordinate b) => new(a.Y + b.Y, a.X + b.X);
    public static Coordinate operator -(Coordinate a, Coordinate b) => new(a.Y - b.Y, a.X - b.X);
    public int ManhattanDistance(Coordinate other) => Math.Abs(Y - other.Y) + Math.Abs(X - other.X);
    public HashSet<Coordinate> GetCoordsWithinManhattanDistance(int distance)
    {
        var coords = new HashSet<Coordinate>();
        for (int y = -distance; y <= distance; y++)
        {
            for (int x = -distance; x <= distance; x++)
            {
                var coord = new Coordinate(Y + y, X + x);
                if (ManhattanDistance(coord) <= distance)
                {
                    coords.Add(coord);
                }
            }
        }
        return coords;
    }

    public Coordinate[] GetNeighbors()
    {
        return [
            new Coordinate(Y - 1, X),
            new Coordinate(Y + 1, X),
            new Coordinate(Y, X - 1),
            new Coordinate(Y, X + 1)
        ];
    }
}