using System.Collections.Frozen;
using System.Collections.Immutable;

var lines = File.ReadAllLines("data.txt");
var trail = new HikingTrail(lines);

var result1 = trail.GetLongestPathByGraph(true);
Console.WriteLine($"Result 1: {result1}");

var result2 = trail.GetLongestPathByGraph(false);
Console.WriteLine($"Result 2: {result2}");


public class HikingTrail
{
    private FrozenDictionary<Tile, char> _tiles;
    private Tile _start;
    private Tile _end;

    public HikingTrail(string[] lines)
    {
        var tiles = new Dictionary<Tile, char>();
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[0].Length; x++)
            {
                tiles.Add(new Tile(y, x), lines[y][x]);
            }
        }
        _start = new Tile(0, 1);
        _end = new Tile(lines.Length - 1, lines[0].Length - 2);
        _tiles = tiles.ToFrozenDictionary();
    }

    public int GetLongestPathByGraph(bool slippery)
    {
        var graph = GetGraph(slippery).ToFrozenDictionary(kvp => kvp.Key, kvp => kvp.Value.ToFrozenDictionary());
        return GetLongestPathInGraph(graph, [], 0, _start);
    }

    private Dictionary<Tile, Dictionary<Tile, int>> GetGraph(bool slippery)
    {
        var graph = new Dictionary<Tile, Dictionary<Tile, int>>();
        var nodes = GetNodes(slippery);
        return nodes.ToDictionary(node => node, node => GetNeighborNodes(nodes, node, slippery));
    }

    private Dictionary<Tile, int> GetNeighborNodes(HashSet<Tile> nodes, Tile node, bool slippery)
    {
        var adjacent = node.Next(_tiles, slippery);
        return adjacent
            .Select(t => GetNextNode(nodes, node, t, 1, slippery))
            .Where(tuple => tuple.Item1 != null && tuple.Item2 != null)
            .ToDictionary(tuple => tuple.Item1!.Value, tuple => tuple.Item2!.Value);
    }

    private (Tile?, int?) GetNextNode(HashSet<Tile> nodes, Tile prev, Tile current, int distanceSoFar, bool slippery)
    {
        if (nodes.Contains(current))
        {
            return (current, distanceSoFar);
        }
        var next = current.Next(_tiles, slippery).Where(t => t != prev).ToArray();
        if (next.Length == 0)
        {
            return (null, null);
        }
        return GetNextNode(nodes, current, next.Single(), distanceSoFar + 1, slippery);
    }

    private int GetLongestPathInGraph(FrozenDictionary<Tile, FrozenDictionary<Tile, int>> graph, ImmutableHashSet<Tile> visited, int currentDistance, Tile current)
    {
        if (current == _end)
        {
            return currentDistance;
        }
        if (visited.Contains(current))
        {
            return int.MinValue;
        }
        var nextNodes = graph[current];
        return nextNodes.Select(kvp => GetLongestPathInGraph(graph, visited.Add(current), currentDistance + kvp.Value, kvp.Key)).Max();
    }

    private HashSet<Tile> GetNodes(bool slippery)
    {
        var nodes = new HashSet<Tile> { _start, _end };
        var maxX = _tiles.MaxBy(t => t.Key.x).Key.x;
        var maxY = _tiles.MaxBy(t => t.Key.y).Key.y;
        for (var y = 0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                var tile = new Tile(y, x);
                if (_tiles[tile] == '.')
                {
                    var next = tile.Next(_tiles, slippery);
                    if (next.Count > 2)
                    {
                        nodes.Add(tile);
                    }
                }
            }
        }
        return nodes;
    }
}

public record struct Tile(int y, int x)
{
    public Tile Left() => this with { x = x - 1 };
    public Tile Right() => this with { x = x + 1 };
    public Tile Up() => this with { y = y - 1 };
    public Tile Down() => this with { y = y + 1 };

    public HashSet<Tile> Next(FrozenDictionary<Tile, char> tiles, bool slippery)
    {
        var type = tiles[this];
        Tile[] candidates = (type, slippery) switch
        {
            ('>', true) => [Right()],
            ('<', true) => [Left()],
            ('v', true) => [Down()],
            ('.', true) => [Right(), Left(), Up(), Down()],
            ('>', false) => [Right(), Left(), Up(), Down()],
            ('<', false) => [Right(), Left(), Up(), Down()],
            ('v', false) => [Right(), Left(), Up(), Down()],
            ('.', false) => [Right(), Left(), Up(), Down()],
            _ => throw new Exception($"Unknown tile type {type}")
        };
        return candidates.Where(candidate =>
        {
            if (tiles.TryGetValue(candidate, out var candidateType))
            {
                if (candidateType == '#')
                {
                    return false;
                }
                return true;
            }
            return false;
        }).ToHashSet();
    }
}