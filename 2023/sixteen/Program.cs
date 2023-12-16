var lines = File.ReadAllLines("data.txt");
var contraption = new Dictionary<Coordinate, char>();
for (var y = 0; y < lines.Length; y++)
{
    for (var x = 0; x < lines[0].Length; x++)
    {
        contraption.Add(new Coordinate(y, x), lines[y][x]);
    }
}

var beams = GetAllBeams(contraption, new HashSet<LightBeam> { new LightBeam(new Coordinate(0, 0), Direction.Right) });
Console.WriteLine($"Result 1: {beams.Select(beam => beam.coordinate).Distinct().Count()}");


int mostSoFar = 0;
for (var i = 0; i < lines.Length; i++)
{
    mostSoFar = new List<int> {
        mostSoFar,
        GetAllBeams(contraption, new HashSet<LightBeam> { new LightBeam(new Coordinate(i, 0), Direction.Right) }).Select(beam => beam.coordinate).Distinct().Count(),
        GetAllBeams(contraption, new HashSet<LightBeam> { new LightBeam(new Coordinate(i, lines[0].Length - 1), Direction.Left) }).Select(beam => beam.coordinate).Distinct().Count(),
        GetAllBeams(contraption, new HashSet<LightBeam> { new LightBeam(new Coordinate(0, i), Direction.Down) }).Select(beam => beam.coordinate).Distinct().Count(),
        GetAllBeams(contraption, new HashSet<LightBeam> { new LightBeam(new Coordinate(lines[0].Length - 1, i), Direction.Up) }).Select(beam => beam.coordinate).Distinct().Count()
    }.Max();
}
Console.WriteLine($"Result 2: {mostSoFar}");

void DrawEnergized(HashSet<Coordinate> coordinates, int maxY, int maxX)
{
    for (var y = -10; y < maxY + 10; y++)
    {
        for (var x = -10; x < maxX + 10; x++)
        {
            if (coordinates.Contains(new Coordinate(y, x)))
            {
                Console.Write('#');
            }
            else
            {
                Console.Write('.');
            }
        }
        Console.WriteLine();
    }
}

HashSet<LightBeam> GetAllBeams(Dictionary<Coordinate, char> contraption, HashSet<LightBeam> start)
{
    HashSet<LightBeam> lightBeams = [];
    HashSet<LightBeam> newLightBeams = start;
    while (true)
    {
        HashSet<LightBeam> nextNewLightBeams = [];
        foreach (var lightBeam in newLightBeams)
        {
            lightBeams.Add(lightBeam);
            if (contraption.TryGetValue(lightBeam.coordinate, out var c))
            {
                var nextBeams = lightBeam.Next(c);
                foreach (var beam in nextBeams)
                {
                    if (contraption.ContainsKey(beam.coordinate) && !lightBeams.Contains(beam))
                    {
                        nextNewLightBeams.Add(beam);
                    }
                }
            }
        }
        if (nextNewLightBeams.Count == 0)
        {
            break;
        }
        newLightBeams = nextNewLightBeams;
    }
    return lightBeams;
}



record LightBeam(Coordinate coordinate, Direction direction)
{
    public LightBeam[] Next(char c) => (direction, c) switch
    {
        (Direction.Up, '.') => [new LightBeam(coordinate with { y = coordinate.y - 1 }, Direction.Up)],
        (Direction.Left, '.') => [new LightBeam(coordinate with { x = coordinate.x - 1 }, Direction.Left)],
        (Direction.Right, '.') => [new LightBeam(coordinate with { x = coordinate.x + 1 }, Direction.Right)],
        (Direction.Down, '.') => [new LightBeam(coordinate with { y = coordinate.y + 1 }, Direction.Down)],
        (Direction.Up, '/') => [new LightBeam(coordinate with { x = coordinate.x + 1 }, Direction.Right)],
        (Direction.Left, '/') => [new LightBeam(coordinate with { y = coordinate.y + 1 }, Direction.Down)],
        (Direction.Right, '/') => [new LightBeam(coordinate with { y = coordinate.y - 1 }, Direction.Up)],
        (Direction.Down, '/') => [new LightBeam(coordinate with { x = coordinate.x - 1 }, Direction.Left)],
        (Direction.Up, '\\') => [new LightBeam(coordinate with { x = coordinate.x - 1 }, Direction.Left)],
        (Direction.Left, '\\') => [new LightBeam(coordinate with { y = coordinate.y - 1 }, Direction.Up)],
        (Direction.Right, '\\') => [new LightBeam(coordinate with { y = coordinate.y + 1 }, Direction.Down)],
        (Direction.Down, '\\') => [new LightBeam(coordinate with { x = coordinate.x + 1 }, Direction.Right)],
        (Direction.Up, '-') => [new LightBeam(coordinate with { x = coordinate.x - 1 }, Direction.Left), new LightBeam(coordinate with { x = coordinate.x + 1 }, Direction.Right)],
        (Direction.Left, '-') => [new LightBeam(coordinate with { x = coordinate.x - 1 }, Direction.Left)],
        (Direction.Right, '-') => [new LightBeam(coordinate with { x = coordinate.x + 1 }, Direction.Right)],
        (Direction.Down, '-') => [new LightBeam(coordinate with { x = coordinate.x - 1 }, Direction.Left), new LightBeam(coordinate with { x = coordinate.x + 1 }, Direction.Right)],
        (Direction.Up, '|') => [new LightBeam(coordinate with { y = coordinate.y - 1 }, Direction.Up)],
        (Direction.Left, '|') => [new LightBeam(coordinate with { y = coordinate.y - 1 }, Direction.Up), new LightBeam(coordinate with { y = coordinate.y + 1 }, Direction.Down)],
        (Direction.Right, '|') => [new LightBeam(coordinate with { y = coordinate.y - 1 }, Direction.Up), new LightBeam(coordinate with { y = coordinate.y + 1 }, Direction.Down)],
        (Direction.Down, '|') => [new LightBeam(coordinate with { y = coordinate.y + 1 }, Direction.Down)],
        _ => throw new Exception($"unhandled case ({direction}, {c})")
    };
}
record Coordinate(int y, int x);
enum Direction
{
    Up,
    Left,
    Right,
    Down
};