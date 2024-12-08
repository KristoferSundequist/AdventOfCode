using System.Numerics;

public class Grid
{
    private Dictionary<char, HashSet<Vector2>> antennasByFrequency;
    private readonly int Height;
    private readonly int Width;

    public Grid(char[][] input)
    {
        antennasByFrequency = ParseInput(input);
        Height = input.Length;
        Width = input[0].Length;
    }

    public HashSet<Vector2> GetAllAntiNodes(IEnumerable<int> distances)
    {
        var antiNodes = new HashSet<Vector2>();
        foreach (var frequency in antennasByFrequency.Keys)
        {
            antiNodes.UnionWith(GetAntiNodesForFrequency(frequency, distances));
        }
        return antiNodes;
    }

    private HashSet<Vector2> GetAntiNodesForFrequency(char frequency, IEnumerable<int> distances)
    {
        var orderedAntennas = antennasByFrequency[frequency].ToArray();
        var antiNodes = new HashSet<Vector2>();
        for (var i = 0; i < orderedAntennas.Length - 1; i++)
        {
            for (var j = i + 1; j < orderedAntennas.Length; j++)
            {
                antiNodes.UnionWith(GetAntiNodes(orderedAntennas[i], orderedAntennas[j], distances));
            }
        }
        return antiNodes;
    }

    private Dictionary<char, HashSet<Vector2>> ParseInput(char[][] input)
    {
        var grid = new Dictionary<char, HashSet<Vector2>>();
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
                    coordinates.Add(new Vector2(x, y));
                }
                else
                {
                    grid[input[y][x]] = new HashSet<Vector2> { new Vector2(x, y) };
                }
            }
        }
        return grid;
    }

    private IEnumerable<Vector2> GetAntiNodes(Vector2 coordinate1, Vector2 coordinate2, IEnumerable<int> distances)
    {
        var angle = coordinate2 - coordinate1;
        var antiNodes = new List<Vector2> { };
        foreach (var distance in distances)
        {
            var diff = angle * distance;
            antiNodes.Add(coordinate1 - diff);
            antiNodes.Add(coordinate2 + diff);
        }
        return antiNodes.Where(c => c.X >= 0 && c.X < Width && c.Y >= 0 && c.Y < Height);
    }
}