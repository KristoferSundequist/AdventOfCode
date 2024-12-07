var lines = File.ReadAllLines("input.txt").Select(line => line.ToCharArray()).ToArray();

Part1();
Part2();

void Part1()
{
    var guard = GetGuard();
    var visited = new HashSet<Coordinate>();
    while (true)
    {
        visited.Add(guard.Position);
        var nextGuard = guard.Advance();
        if (nextGuard.Position.Y < 0 || nextGuard.Position.Y >= lines.Length || nextGuard.Position.X < 0 || nextGuard.Position.X >= lines[0].Length)
        {
            break;
        }
        if (lines[nextGuard.Position.Y][nextGuard.Position.X] == '#')
        {
            guard = guard.TurnRight();
        }
        else
        {
            guard = nextGuard;
        }
    }
    Console.WriteLine($"Part1: {visited.Count}");
}

void Part2()
{
    var loopCount = 0;
    for (var y = 0; y < lines.Length; y++)
    {
        for (var x = 0; x < lines[y].Length; x++)
        {
            if (lines[y][x] == '.')
            {
                if (CheckWithObstruction(new Coordinate(y, x)))
                {
                    loopCount++;
                }
            }
        }
    }
    Console.WriteLine($"Part2: {loopCount}");
}

char[][] GetLinesWithObstruction(Coordinate newObstruction)
{
    var linesWithObs = lines.Select(line => line.ToArray()).ToArray();
    linesWithObs[newObstruction.Y][newObstruction.X] = '#';
    return linesWithObs;
}

bool CheckWithObstruction(Coordinate newObstruction)
{
    var linesWithObs = GetLinesWithObstruction(newObstruction);
    var guard = GetGuard();

    var visited = new HashSet<Guard>();
    while (true)
    {
        if (visited.Contains(guard))
        {
            return true;
        }
        visited.Add(guard);
        var nextGuard = guard.Advance();
        if (nextGuard.Position.Y < 0 || nextGuard.Position.Y >= linesWithObs.Length || nextGuard.Position.X < 0 || nextGuard.Position.X >= linesWithObs[0].Length)
        {
            return false;
        }
        if (linesWithObs[nextGuard.Position.Y][nextGuard.Position.X] == '#')
        {
            guard = guard.TurnRight();
        }
        else
        {
            guard = nextGuard;
        }
    }
}

Guard GetGuard()
{
    for (var y = 0; y < lines.Length; y++)
    {
        for (var x = 0; x < lines[y].Length; x++)
        {
            if (lines[y][x] == '^')
            {
                return new Guard(new Coordinate(y, x), Direction.Up);
            }
        }
    }
    throw new Exception("No position found");
}

record Guard(Coordinate Position, Direction Direction)
{
    public Guard Advance()
    {
        return Direction switch
        {
            Direction.Up => new Guard(new Coordinate(Position.Y - 1, Position.X), Direction),
            Direction.Down => new Guard(new Coordinate(Position.Y + 1, Position.X), Direction),
            Direction.Left => new Guard(new Coordinate(Position.Y, Position.X - 1), Direction),
            Direction.Right => new Guard(new Coordinate(Position.Y, Position.X + 1), Direction),
            _ => throw new Exception("Invalid direction")
        };
    }

    public Guard TurnRight()
    {
        return Direction switch
        {
            Direction.Up => this with { Direction = Direction.Right },
            Direction.Right => this with { Direction = Direction.Down },
            Direction.Down => this with { Direction = Direction.Left },
            Direction.Left => this with { Direction = Direction.Up },
            _ => throw new Exception("Invalid direction")
        };
    }
}
enum Direction
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3
};
record Coordinate(int Y, int X);