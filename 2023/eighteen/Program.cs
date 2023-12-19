var lines = File.ReadAllLines("./testdata.txt");
var instructions = lines.Select(Instruction.FromString).ToArray();
var plan = Plan.FromInstructions(instructions);
//plan.Draw(false);
var result1 = plan.GetAreaFloodFillPlusBorder();
var resultBad = plan.GetInsideArea() + plan.Holes.Count();
//var result1real = plan.GetArea();
//var polygonArea = plan.CalculateArea();
Console.WriteLine($"Result 1: {result1}");
Console.WriteLine($"Result bad: {resultBad}");
//Console.WriteLine($"Result 1 polygonArea: {polygonArea}");
//plan.Draw(false);

var instructions2 = instructions.Select(instruction => Instruction.FromColor(instruction.color)).ToArray();
var plan2 = Plan.FromInstructions(instructions2);
//Console.WriteLine(plan2.Holes.Count);

Console.WriteLine(plan2.MinY);
Console.WriteLine(plan2.MaxY);

var result2 = plan2.GetInsideArea() + plan2.Holes.Count;
Console.WriteLine($"Result 2: {result2}");

public class Plan
{
    public required HashSet<Coordinate> Holes { get; init; }
    public required List<Coordinate> Corners { get; init; }
    public required Surrounding TheSurrounding { get; init; }

    public required int MaxY { get; init; }
    public required int MaxX { get; init; }
    public required int MinY { get; init; }
    public required int MinX { get; init; }

    public required Dictionary<int, List<int>> Verticals { get; init; }
    public required HashSet<Coordinate> HorizontalLineStarts { get; init; }


    public static Plan FromInstructions(IEnumerable<Instruction> instructions)
    {
        var start = new Coordinate(0, 0);
        var current = start;
        var holes = new HashSet<Coordinate>();
        var corners = new List<Coordinate>();

        var verticals = new Dictionary<int, HashSet<int>>();
        var horizontalLineStarts = new HashSet<Coordinate>();


        foreach (var instruction in instructions)
        {
            if (instruction.direction == 'R')
            {
                horizontalLineStarts.Add(current);
            }
            corners.Add(current);
            var change = instruction.direction switch
            {
                'R' => new Coordinate(0, 1),
                'D' => new Coordinate(1, 0),
                'L' => new Coordinate(0, -1),
                'U' => new Coordinate(-1, 0),
                _ => throw new Exception("unknown direction")
            };
            for (var i = 0; i < instruction.length; i++)
            {
                if (instruction.direction == 'U' || instruction.direction == 'D')
                {
                    if (verticals.TryGetValue(current.y, out var prevList))
                    {
                        prevList.Add(current.x);
                    }
                    else
                    {
                        verticals.Add(current.y, [current.x]);
                    }
                }
                current += change;
                if (instruction.direction == 'U' || instruction.direction == 'D')
                {
                    if (verticals.TryGetValue(current.y, out var prevList))
                    {
                        prevList.Add(current.x);
                    }
                    else
                    {
                        verticals.Add(current.y, [current.x]);
                    }
                }
                holes.Add(current);
            }
            if (instruction.direction == 'L')
            {
                horizontalLineStarts.Add(current);
            }
        }
        var maxY = holes.MaxBy(coord => coord.y).y + 1;
        var maxX = holes.MaxBy(coord => coord.x).x + 1;
        var minY = holes.MinBy(coord => coord.y).y;
        var minX = holes.MinBy(coord => coord.x).x;
        var sortedVerticals = verticals.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
        foreach (var kvp in sortedVerticals)
        {
            kvp.Value.Sort();
        }
        corners.Add(start);
        return new Plan
        {
            Holes = holes,
            MaxY = maxY,
            MaxX = maxX,
            MinY = minY,
            MinX = minX,
            Corners = corners,
            TheSurrounding = GetSurroundings(holes),
            Verticals = sortedVerticals,
            HorizontalLineStarts = horizontalLineStarts
        };
    }

    public long GetInsideArea()
    {
        long area = 0;
        for (var y = MinY; y < MaxY - 1; y++)
        {
            var verticals = Verticals[y];
            for (var i = 0; i < verticals.Count - 1; i++)
            {
                var nextCoord = new Coordinate(y, verticals[i] + 1);
                if (TheSurrounding.inside.Contains(nextCoord))
                {
                    area += verticals[i + 1] - verticals[i] - 1;
                }
            }
        }
        return area;
    }

    public double CalculateArea()
    {
        double area = 0;

        for (int i = 0; i < Corners.Count - 1; i++)
        {
            var first = GetAdjustedCoordinate(Corners[i]);
            var second = GetAdjustedCoordinate(Corners[i + 1]);
            //var first = Corners[i];
            //var second = Corners[i+1];
            area += -first.y * second.x + first.x * second.y;
        }
        return 0.5 * Math.Abs(area);
    }

    private Coordinate GetAdjustedCoordinate(Coordinate corner)
    {
        var bottomLeft = corner + new Coordinate(1, -1);
        var topLeft = corner + new Coordinate(-1, -1);
        var bottomRight = corner + new Coordinate(1, 1);
        var topRight = corner + new Coordinate(-1, 1);
        var right = corner + new Coordinate(0, 1);
        var top = corner + new Coordinate(-1, 0);
        var left = corner + new Coordinate(0, -1);
        var down = corner + new Coordinate(1, 0);
        if (TheSurrounding.inside.Contains(bottomLeft))
        {
            return right;
        }
        if (TheSurrounding.inside.Contains(topRight))
        {
            return down;
        }
        if (TheSurrounding.inside.Contains(bottomRight))
        {
            return corner;
        }
        if (TheSurrounding.inside.Contains(topLeft))
        {
            return bottomRight;
        }
        Console.WriteLine(corner);
        throw new Exception("Unexpected no case");
    }

    public record Surrounding(HashSet<Coordinate> inside, HashSet<Coordinate> outside);
    private static Surrounding GetSurroundings(HashSet<Coordinate> holes)
    {
        Console.WriteLine("Calculating surroundings..");
        var (start1, start2) = GetStarts(holes);
        var edges1 = GetEdge(holes, start1);
        var edges2 = GetEdge(holes, start2);
        Console.WriteLine("Surroundings completed.");
        if (edges1.Count < edges2.Count)
        {
            return new Surrounding(edges1, edges2);
        }
        else
        {
            return new Surrounding(edges2, edges1);
        }
    }

    private static HashSet<Coordinate> GetEdge(HashSet<Coordinate> holes, Coordinate start)
    {
        var found = new HashSet<Coordinate>();
        var next = new HashSet<Coordinate> { start };

        while (next.Count != 0)
        {
            var nextnext = new HashSet<Coordinate>();
            foreach (var coord in next)
            {
                var adjacent = coord.GetAdjacentCoordinates();
                foreach (var adjacentCoord in adjacent)
                {
                    if (!holes.Contains(adjacentCoord) && !found.Contains(adjacentCoord) && adjacentCoord.GetAdjacentCoordinates().Any(coord => holes.Contains(coord)))
                    {
                        found.Add(adjacentCoord);
                        nextnext.Add(adjacentCoord);
                    }
                }
                next = nextnext;
            }
        }
        return found;
    }

    private static (Coordinate, Coordinate) GetStarts(HashSet<Coordinate> holes)
    {
        foreach (var hole in holes)
        {
            var up = hole with { y = hole.y - 1 };
            var down = hole with { y = hole.y + 1 };
            if (!holes.Contains(up) && !holes.Contains(down))
            {
                return (up, down);
            }
            var left = hole with { x = hole.x - 1 };
            var right = hole with { x = hole.x + 1 };
            if (!holes.Contains(left) && !holes.Contains(right))
            {
                return (left, right);
            }
        }
        throw new Exception("Couldnt find starts");
    }

    public FloodResult GetInteriorFloodFill(Coordinate start)
    {
        var found = new HashSet<Coordinate>();
        var next = new HashSet<Coordinate> { start };

        while (next.Count != 0)
        {
            var nextnext = new HashSet<Coordinate>();
            foreach (var coord in next)
            {
                var adjacent = coord.GetAdjacentCoordinates();
                foreach (var adjacentCoord in adjacent)
                {
                    if (adjacentCoord.x > MaxX)
                    {
                        return new FloodResult(false, new());
                    }
                    if (!Holes.Contains(adjacentCoord) && !found.Contains(adjacentCoord))
                    {
                        found.Add(adjacentCoord);
                        nextnext.Add(adjacentCoord);
                    }
                }
                next = nextnext;
            }
        }
        return new FloodResult(true, found);
    }

    public long GetAreaFloodFillPlusBorder()
    {
        var result = GetInteriorFloodFill(new Coordinate(1, 1));
        if (!result.isInterior)
        {
            throw new Exception("fail get interior");
        }
        return result.coordinates.Count + Holes.Count;
    }

    public void Draw(bool showInterior, bool showSurroundings = false)
    {
        var result = showInterior ? GetInteriorFloodFill(new Coordinate(1, 1)) : new FloodResult(false, new());
        var interior = result.coordinates;
        for (var y = MinY - 5; y < MaxY + 5; y++)
        {
            for (var x = MinX - 5; x < MaxX + 5; x++)
            {
                var coord = new Coordinate(y, x);
                if (Holes.Contains(coord))
                {
                    Console.Write("#");
                }
                else if (showSurroundings && TheSurrounding.inside.Contains(coord))
                {
                    Console.Write("%");
                }
                else if (showSurroundings && TheSurrounding.outside.Contains(coord))
                {
                    Console.Write("¤");
                }
                else if (interior.Contains(coord))
                {
                    Console.Write("@");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.Write($" --- {y}");
            Console.WriteLine("");
        }
    }

}

public record FloodResult(bool isInterior, HashSet<Coordinate> coordinates);
public record Instruction(char direction, int length, string color)
{
    public static Instruction FromString(string str)
    {
        var parts = str.Split(" ");
        if (parts is [string dir, string length, string color])
        {
            return new Instruction(dir[0], int.Parse(length), color[2..^1]);
        }
        throw new Exception("unexapowec parse error");
    }

    public static Instruction FromColor(string color)
    {
        var newDirection = color[^1] switch
        {
            '0' => 'R',
            '1' => 'D',
            '2' => 'L',
            '3' => 'U',
            _ => throw new Exception("from color direction parse fail")
        };
        var newLength = Convert.ToInt32(color[..^1], 16);
        return new Instruction(newDirection, newLength, "");
    }
}
public record struct Coordinate(int y, int x)
{
    public static Coordinate operator +(Coordinate a, Coordinate b) => new Coordinate(a.y + b.y, a.x + b.x);
    public HashSet<Coordinate> GetAdjacentCoordinates()
    {
        return new HashSet<Coordinate> {
            this with { x = this.x - 1, y = this.y - 1 },
            this with { x = this.x, y = this.y - 1 },
            this with { x = this.x + 1, y = this.y - 1 },
            this with { x = this.x - 1, y = this.y },
            this with { x = this.x + 1, y = this.y },
            this with { x = this.x - 1, y = this.y + 1 },
            this with { x = this.x, y = this.y + 1 },
            this with { x = this.x + 1, y = this.y + 1 },
        };
    }
}