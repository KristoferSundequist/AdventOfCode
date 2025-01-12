using System.Collections.Frozen;

public class Maze
{
    private Dictionary<Coordinate, char> _map = new();
    private Reindeer _reindeerStart = new Reindeer(Direction.East, Coordinate.Zero);
    private Coordinate _target = Coordinate.Zero;

    public Maze(char[][] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                var coord = new Coordinate(y, x);
                if (lines[y][x] == 'E')
                {
                    _target = coord;
                    _map[coord] = '.';
                }
                else if (lines[y][x] == 'S')
                {
                    _map[coord] = '.';
                    _reindeerStart = _reindeerStart with { position = coord };
                }
                else
                {
                    _map[coord] = lines[y][x];
                }
            }
        }

        if (_reindeerStart.position == Coordinate.Zero)
        {
            throw new InvalidOperationException("No starting position found");
        }

        if (_target == Coordinate.Zero)
        {
            throw new InvalidOperationException("No target found");
        }

    }

    public long GetLowestScore()
    {
        var shortestPaths = GetShortestPaths();
        return shortestPaths.Where(x => x.Key.position == _target).Select(x => x.Value).Min();
    }

    public long GetNumTilesInShortestPaths()
    {
        var shortestPaths = GetShortestPaths();
        var lowestScore = shortestPaths.Where(x => x.Key.position == _target).Select(x => x.Value).Min();
        var frontier = shortestPaths.Where(x => x.Key.position == _target && x.Value == lowestScore);

        var tilesInShortestPaths = new HashSet<Coordinate> { _target };
        foreach (var (reindeer, cost) in frontier)
        {
            PopulateTilesInShortestPaths(shortestPaths.ToFrozenDictionary(), reindeer, cost, tilesInShortestPaths);
        }
        //Print(tilesInShortestPaths);
        return tilesInShortestPaths.Count;
    }

    private void PopulateTilesInShortestPaths(FrozenDictionary<Reindeer, long> shortestPaths, Reindeer currentReindeer, long currentCost, HashSet<Coordinate> tilesInShortestPaths)
    {
        var backwardMoves = GetPossibleBackwardMovesWithCost(currentReindeer);

        foreach (var (newReindeer, cost) in backwardMoves)
        {
            if (shortestPaths.TryGetValue(newReindeer, out var shortestCost))
            {
                var newCost = currentCost - cost;
                if (shortestCost == newCost)
                {
                    tilesInShortestPaths.Add(newReindeer.position);
                    PopulateTilesInShortestPaths(shortestPaths, newReindeer, newCost, tilesInShortestPaths);
                }
            }
        }
    }

    private Dictionary<Reindeer, long> GetShortestPaths()
    {
        var lowestFound = new Dictionary<Reindeer, long> { { _reindeerStart, 0 } };

        var frontier = new Dictionary<Reindeer, long> {
            {_reindeerStart, 0}
        };
        while (frontier.Count > 0)
        {
            var newFrontier = new Dictionary<Reindeer, long>();

            foreach (var (reindeer, currenctCost) in frontier)
            {
                var possibleMovesWithCost = GetPossibleMovesWithCost(reindeer);
                foreach (var (movedReindeer, moveCost) in possibleMovesWithCost)
                {
                    var newCost = currenctCost + moveCost;
                    if (lowestFound.TryGetValue(movedReindeer, out var currentLowest))
                    {
                        if (newCost < currentLowest)
                        {
                            lowestFound[movedReindeer] = newCost;
                            newFrontier[movedReindeer] = newCost;
                        }
                    }
                    else
                    {
                        lowestFound[movedReindeer] = newCost;
                        newFrontier[movedReindeer] = newCost;
                    }
                }
            }
            frontier = newFrontier;
        }

        return lowestFound;
    }

    public void Print(HashSet<Coordinate> highLights)
    {
        var maxY = _map.Keys.Max(x => x.Y);
        var maxX = _map.Keys.Max(x => x.X);
        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                var coord = new Coordinate(y, x);
                if (highLights.Contains(coord))
                {
                    Console.Write('O');
                }
                else if (_reindeerStart.position == coord)
                {
                    Console.Write('S');
                }
                else if (_target == coord)
                {
                    Console.Write('E');
                }
                else
                {
                    Console.Write(_map[coord]);
                }
            }
            Console.WriteLine();
        }
    }

    private Dictionary<Reindeer, long> GetPossibleMovesWithCost(Reindeer reindeer)
    {
        var result = new Dictionary<Reindeer, long>();
        result[reindeer.TurnLeft()] = 1000;
        result[reindeer.TurnRight()] = 1000;

        var forward = reindeer.MoveForward();
        if (_map.TryGetValue(forward.position, out var value) && value != '#')
        {
            result[forward] = 1;
        }

        return result;
    }

    private Dictionary<Reindeer, long> GetPossibleBackwardMovesWithCost(Reindeer reindeer)
    {
        var result = new Dictionary<Reindeer, long>();
        result[reindeer.TurnLeft()] = 1000;

        result[reindeer.TurnRight()] = 1000;

        var backward = reindeer.MoveBackward();
        if (_map.TryGetValue(backward.position, out var value) && value != '#')
        {
            result[backward] = 1;
        }

        return result;
    }
}