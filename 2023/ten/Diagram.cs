
using System.Collections.Frozen;

public record Coordinate(int y, int x);
public class Diagram
{
    private Dictionary<Coordinate, char> _tiles = new();
    private Coordinate StartingCoord = new Coordinate(-1, -1);
    private int maxX = -1;
    private int maxY = -1;

    public Diagram(string[] lines, bool expanded)
    {
        if (expanded)
        {
            SetExpandedTiles(lines);
        }
        else
        {
            SetNormalTiles(lines);
        }
    }

    private void SetNormalTiles(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                var coord = new Coordinate(y, x);
                _tiles.Add(coord, lines[y][x]);
                if (lines[y][x] == 'S')
                {
                    StartingCoord = coord;
                }
            }
        }
    }

    private void SetExpandedTiles(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                var c = lines[y][x];
                var originalCoord = new Coordinate(y, x);
                var coord = originalCoord with { x = originalCoord.x * 2, y = originalCoord.y * 2 };

                var west = coord with { x = coord.x - 1 };
                var east = coord with { x = coord.x + 1 };
                var north = coord with { y = coord.y - 1 };
                var south = coord with { y = coord.y + 1 };
                var southEast = coord with { y = coord.y + 1, x = coord.x + 1 };

                _tiles.Add(coord, c);
                if (c == 'L')
                {
                    _tiles.Add(east, '-');
                }
                else if (c == 'J')
                {

                }
                else if (c == '7')
                {
                    _tiles.Add(south, '|');
                }
                else if (c == 'F')
                {
                    _tiles.Add(south, '|');
                    _tiles.Add(east, '-');
                }
                else if (c == '|')
                {
                    _tiles.Add(south, '|');
                }
                else if (c == '-')
                {
                    _tiles.Add(east, '-');
                }
                else if (c == 'S')
                {
                    StartingCoord = coord;
                    if (new List<char> { '-', 'J', '7' }.Contains(lines[y][x + 1]))
                    {
                        _tiles.Add(east, '-');
                    }
                    if (new List<char> { '|', 'L', 'J' }.Contains(lines[y + 1][x]))
                    {
                        _tiles.Add(south, '|');
                    }
                }

                if (!_tiles.ContainsKey(south))
                {
                    _tiles.Add(south, '.');
                }
                if (!_tiles.ContainsKey(southEast))
                {
                    _tiles.Add(southEast, '.');
                }
                if (!_tiles.ContainsKey(east))
                {
                    _tiles.Add(east, '.');
                }
            }
        }
        maxY = lines.Length * 2;
        maxX = lines[0].Length * 2;
    }

    public long GetFurthestDistanceFromStart()
    {
        return GetLoopCoordinates().MaxBy(kvp => kvp.Value).Value;
    }

    private HashSet<Coordinate> GetEnclosed()
    {
        var loopCoordinates = GetLoopCoordinates().Select(kvp => kvp.Key).ToFrozenSet();
        var enclosed = new HashSet<Coordinate>();
        for (var y = 0; y < maxY; y += 2)
        {
            Console.WriteLine($"{y} / {maxY}");
            for (var x = 0; x < maxX; x += 2)
            {
                var coord = new Coordinate(y, x);
                var result = IsEnclosed(coord, loopCoordinates);
                if (result.isEnclosed)
                {
                    enclosed.Add(coord);
                }
            }
        }
        return enclosed;
    }

    public long GetNumEnclosed() => GetEnclosed().Count();

    public void Draw()
    {
        var loopCoordinates = GetLoopCoordinates().Select(kvp => kvp.Key).ToFrozenSet();
        var enclosed = GetEnclosed();
        for (var y = 0; y < maxY; y++)
        {
            for (var x = 0; x < maxX; x++)
            {
                var coord = new Coordinate(y, x);
                Console.Write(_tiles[coord]);
                continue;
                if (loopCoordinates.Contains(coord))
                {
                    Console.Write('?');
                }
                else if (enclosed.Contains(coord))
                {
                    Console.Write('I');
                }
                else
                {
                    Console.Write('O');
                }
            }
            Console.WriteLine("");
        }
    }

    private EnclosedResult IsEnclosed(Coordinate coord, FrozenSet<Coordinate> loopCoordinates)
    {
        if (loopCoordinates.Contains(coord))
        {
            return new EnclosedResult(false, new HashSet<Coordinate> { coord });
        }
        var visited = new HashSet<Coordinate>();
        var currentCoordinates = new HashSet<Coordinate> { coord };

        while (currentCoordinates.Count > 0)
        {
            var nextCoordinates = new HashSet<Coordinate>();
            foreach (var coordinate in currentCoordinates)
            {
                if (!visited.Contains(coordinate))
                {
                    visited.Add(coordinate);
                    var adjacentCoordinates = GetAdjacentCoordinates(coordinate);
                    if (adjacentCoordinates.Any(coord => IsOutside(coord)))
                    {
                        return new EnclosedResult(false, visited);
                    }
                    nextCoordinates = nextCoordinates.Union(adjacentCoordinates).Where(coord => !loopCoordinates.Contains(coord)).ToHashSet();
                }
            }
            currentCoordinates = nextCoordinates;
        }
        return new EnclosedResult(true, visited);
    }
    private record EnclosedResult(bool isEnclosed, HashSet<Coordinate> visited);

    private bool IsOutside(Coordinate coordinate) => coordinate.x < 0 || coordinate.x >= maxX || coordinate.y < 0 || coordinate.y >= maxY;

    private Dictionary<Coordinate, long> GetLoopCoordinates()
    {
        var visitedAt = new Dictionary<Coordinate, long>();
        var currentCoordinates = new HashSet<Coordinate> { StartingCoord };

        long iteration = 0;
        while (currentCoordinates.Count > 0)
        {
            var nextCoordinates = new HashSet<Coordinate>();
            foreach (var coordinate in currentCoordinates)
            {
                if (!visitedAt.ContainsKey(coordinate))
                {
                    visitedAt.Add(coordinate, iteration);
                    if (TryGetConnectedCoords(coordinate, out var connectedCoordinates))
                    {
                        nextCoordinates = nextCoordinates.Union(connectedCoordinates).ToHashSet();
                    }
                }
            }
            iteration++;
            currentCoordinates = nextCoordinates;
        }
        return visitedAt;
    }

    private static HashSet<Coordinate> GetAdjacentCoordinates(Coordinate coord)
    {
        return new HashSet<Coordinate> {
            coord with { x = coord.x - 1, y = coord.y - 1 },
            coord with { x = coord.x, y = coord.y - 1 },
            coord with { x = coord.x + 1, y = coord.y - 1 },
            coord with { x = coord.x - 1, y = coord.y },
            coord with { x = coord.x + 1, y = coord.y },
            coord with { x = coord.x - 1, y = coord.y + 1 },
            coord with { x = coord.x, y = coord.y + 1 },
            coord with { x = coord.x + 1, y = coord.y + 1 },
        };
    }

    private bool TryGetConnectedCoords(Coordinate coord, out HashSet<Coordinate> connectedCoordinates)
    {
        connectedCoordinates = new();
        var west = coord with { x = coord.x - 1 };
        var east = coord with { x = coord.x + 1 };
        var north = coord with { y = coord.y - 1 };
        var south = coord with { y = coord.y + 1 };

        if (_tiles.TryGetValue(coord, out var tile))
        {
            if (tile == '|')
            {
                connectedCoordinates.Add(north);
                connectedCoordinates.Add(south);
            }
            else if (tile == '-')
            {
                connectedCoordinates.Add(west);
                connectedCoordinates.Add(east);
            }
            else if (tile == 'L')
            {
                connectedCoordinates.Add(north);
                connectedCoordinates.Add(east);
            }
            else if (tile == 'J')
            {
                connectedCoordinates.Add(north);
                connectedCoordinates.Add(west);
            }
            else if (tile == '7')
            {
                connectedCoordinates.Add(south);
                connectedCoordinates.Add(west);
            }
            else if (tile == 'F')
            {
                connectedCoordinates.Add(south);
                connectedCoordinates.Add(east);
            }
            else if (tile == '.')
            {
                // nothing connected
            }
            else if (tile == 'S')
            {
                foreach (var direction in new Coordinate[] { west, east, north, south })
                {
                    if (TryGetConnectedCoords(direction, out var connected) && connected.Contains(coord))
                    {
                        connectedCoordinates.Add(direction);
                    }
                }
            }
            return true;
        }
        //Console.WriteLine($"trying to access outside of diagram: {coord}");
        return false;
    }
}