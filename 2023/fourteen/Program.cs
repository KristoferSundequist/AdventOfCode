var lines = File.ReadAllLines("data.txt");
var platform = Platform.FromLines(lines);

var platformPostNorthTilt = platform.RollDirection(Direction.North);
var result1 = platformPostNorthTilt.GetScore();
Console.WriteLine($"Result 1: {result1}");

var platformPostCycle = platform;
var platformDict = new Dictionary<HashSet<Coordinate>, int>(HashSet<Coordinate>.CreateSetComparer());
var goal = 1000000000;
for (var i = 0; i < goal; i++)
{
    if (platformDict.TryGetValue(platformPostCycle.RoundRocks, out var lastIteration))
    {
        var cycleLength = i - lastIteration;
        Console.WriteLine($"Found a cycle at iteration {i} with length {cycleLength}");
        var iterationsLeft = goal - i;
        var skips = iterationsLeft / cycleLength;
        var skippedIterations = cycleLength * skips;
        for (var j = i + skippedIterations; j < goal; j++)
        {
            platformPostCycle = platformPostCycle.RollCycle();
        }
        break;
    }
    platformDict.Add(platformPostCycle.RoundRocks, i);
    platformPostCycle = platformPostCycle.RollCycle();
}
var result2 = platformPostCycle.GetScore();
Console.WriteLine($"Result 2: {result2}");

public record Platform
{
    public required HashSet<Coordinate> RoundRocks { get; set; }
    public required HashSet<Coordinate> SquareRocks { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }

    public static Platform FromLines(string[] lines)
    {
        var roundRocks = new HashSet<Coordinate>();
        var squareRocks = new HashSet<Coordinate>();
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[0].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    squareRocks.Add(new Coordinate(y, x));
                }
                if (lines[y][x] == 'O')
                {
                    roundRocks.Add(new Coordinate(y, x));
                }
            }
        }
        return new Platform { RoundRocks = roundRocks, SquareRocks = squareRocks, Height = lines.Length, Width = lines[0].Length };
    }

    public int GetScore() => RoundRocks.Sum(coordinate => Height - coordinate.Y);

    public Platform RollDirection(Direction direction)
    {
        var currentRoundRocks = RoundRocks;
        while (true)
        {
            var nextRoundRocks = RollStep(currentRoundRocks, direction);
            if (nextRoundRocks.SetEquals(currentRoundRocks))
            {
                return this with { RoundRocks = nextRoundRocks };
            }
            currentRoundRocks = nextRoundRocks;
        }
    }

    public Platform RollCycle()
    {
        return RollDirection(Direction.North).RollDirection(Direction.West).RollDirection(Direction.South).RollDirection(Direction.East);
    }

    public HashSet<Coordinate> RollStep(HashSet<Coordinate> roundRocks, Direction direction)
    {
        var newRoundRocks = new HashSet<Coordinate>();
        foreach (var roundRock in roundRocks)
        {
            var candidateCoordinate = roundRock.Rolled(direction);
            var isOutOfBounds = candidateCoordinate.Y < 0 || candidateCoordinate.X < 0 || candidateCoordinate.Y >= Height || candidateCoordinate.X >= Width;
            if (isOutOfBounds || roundRocks.Contains(candidateCoordinate) || SquareRocks.Contains(candidateCoordinate))
            {
                newRoundRocks.Add(roundRock);
            }
            else
            {
                newRoundRocks.Add(candidateCoordinate);
            }
        }
        return newRoundRocks;
    }

    public void Draw()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var coord = new Coordinate(y, x);
                if (RoundRocks.Contains(coord))
                {
                    Console.Write("O");
                }
                else if (SquareRocks.Contains(coord))
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
    }
}

public record Coordinate(int Y, int X)
{
    public Coordinate Rolled(Direction direction) => direction switch
    {
        Direction.North => this with { Y = this.Y - 1 },
        Direction.East => this with { X = this.X + 1 },
        Direction.South => this with { Y = this.Y + 1 },
        Direction.West => this with { X = this.X - 1 },
        _ => throw new Exception("Invalid direction")
    };
}
public enum Direction
{
    North,
    East,
    South,
    West
}