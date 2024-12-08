public class Grid
{
    private Dictionary<char, HashSet<Vec2>> antennasByFrequency;
    private readonly int Height;
    private readonly int Width;

    public Grid(char[][] input)
    {
        antennasByFrequency = ParseInput(input);
        Height = input.Length;
        Width = input[0].Length;
    }

    public HashSet<Vec2> GetAllAntiNodes(IEnumerable<int> distances)
    {
        var antiNodes = new HashSet<Vec2>();
        foreach (var frequency in antennasByFrequency.Keys)
        {
            antiNodes.UnionWith(GetAntiNodesForFrequency(frequency, distances));
        }
        return antiNodes;
    }

    private HashSet<Vec2> GetAntiNodesForFrequency(char frequency, IEnumerable<int> distances)
    {
        var orderedAntennas = antennasByFrequency[frequency].ToArray();
        var antiNodes = new HashSet<Vec2>();
        for (var i = 0; i < orderedAntennas.Length - 1; i++)
        {
            for (var j = i + 1; j < orderedAntennas.Length; j++)
            {
                antiNodes.UnionWith(GetAntiNodes(orderedAntennas[i], orderedAntennas[j], distances));
            }
        }
        return antiNodes;
    }

    private Dictionary<char, HashSet<Vec2>> ParseInput(char[][] input)
    {
        var grid = new Dictionary<char, HashSet<Vec2>>();
        for (var y = 0; y < input.Length; y++)
        {
            for (var x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '.')
                {
                    continue;
                }

                if (grid.TryGetValue(input[y][x], out var coordinates))
                {
                    coordinates.Add(new Vec2(x, y));
                }
                else
                {
                    grid[input[y][x]] = new HashSet<Vec2> { new Vec2(x, y) };
                }
            }
        }
        return grid;
    }

    private IEnumerable<Vec2> GetAntiNodes(Vec2 coordinate1, Vec2 coordinate2, IEnumerable<int> distances)
    {
        var angle = coordinate2.Subtract(coordinate1);
        var antiNodes = new List<Vec2> { };
        foreach (var distance in distances)
        {
            var diff = angle.Multiply(distance);
            antiNodes.Add(coordinate1.Subtract(diff));
            antiNodes.Add(coordinate2.Add(diff));
        }
        return antiNodes.Where(c => c.X >= 0 && c.X < Width && c.Y >= 0 && c.Y < Height);
    }
}