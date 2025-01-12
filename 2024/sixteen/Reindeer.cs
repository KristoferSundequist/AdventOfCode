public record struct Reindeer(Direction direction, Coordinate position)
{
    public Reindeer TurnLeft() => direction switch
    {
        Direction.North => this with { direction = Direction.West },
        Direction.East => this with { direction = Direction.North },
        Direction.South => this with { direction = Direction.East },
        Direction.West => this with { direction = Direction.South },
        _ => throw new InvalidOperationException()
    };

    public Reindeer TurnRight() => direction switch
    {
        Direction.North => this with { direction = Direction.East },
        Direction.East => this with { direction = Direction.South },
        Direction.South => this with { direction = Direction.West },
        Direction.West => this with { direction = Direction.North },
        _ => throw new InvalidOperationException()
    };

    public Reindeer MoveForward() => this with { position = position.Move(direction) };
    public Reindeer MoveBackward() => this with { position = position.Move(direction.Opposite()) };
}

public enum Direction
{
    North,
    East,
    South,
    West
}

public static class DirectionMethods
{
    public static Direction Opposite(this Direction direction) => direction switch
    {
        Direction.North => Direction.South,
        Direction.East => Direction.West,
        Direction.South => Direction.North,
        Direction.West => Direction.East,
        _ => throw new InvalidOperationException()
    };
}