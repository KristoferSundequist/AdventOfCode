using System.Text;

public record Coordinate(int Y, int X)
{
    public static Coordinate operator +(Coordinate a, Coordinate b) => new(a.Y + b.Y, a.X + b.X);
    public static Coordinate operator -(Coordinate a, Coordinate b) => new(a.Y - b.Y, a.X - b.X);

    public Coordinate Move(char direction)
    {
        return direction switch
        {
            '^' => new Coordinate(Y - 1, X),
            'v' => new Coordinate(Y + 1, X),
            '<' => new Coordinate(Y, X - 1),
            '>' => new Coordinate(Y, X + 1),
            char c => throw new ArgumentException($"Invalid direction: {c}"),
        };
    }

}