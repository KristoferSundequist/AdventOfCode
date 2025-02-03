using System.Collections.Immutable;

public class Map
{
    private HashSet<Coordinate> _obstacles = [];
    private HashSet<Coordinate> _nonObstacles = new();
    private Coordinate start;
    private Coordinate end;

    public Map(string[][] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == "#")
                {
                    _obstacles.Add(new Coordinate(x, y));
                }
                else if (lines[y][x] == "S")
                {
                    _nonObstacles.Add(new Coordinate(x, y));
                    start = new Coordinate(x, y);
                }
                else if (lines[y][x] == "E")
                {
                    _nonObstacles.Add(new Coordinate(x, y));
                    end = new Coordinate(x, y);
                }
                else if (lines[y][x] == ".")
                {
                    _nonObstacles.Add(new Coordinate(x, y));
                }
                else
                {
                    throw new Exception($"Unknown character {lines[y][x]} at ({x}, {y})");
                }
            }
        }
    }

    public Dictionary<int, int> NumCheatsPerSaved(int cheatDistance)
    {
        var distances = GetDistances();
        var numCheatsPerSavedSeconds = new Dictionary<int, int>();

        foreach (var nonObstacle in _nonObstacles)
        {
            var coordsWithinRadius = nonObstacle
                .GetCoordsWithinManhattanDistance(cheatDistance)
                .Where(c => _nonObstacles.Contains(c));
            
            foreach (var cheatTarget in coordsWithinRadius)
            {
                var numSecondsSaved = distances[nonObstacle] - distances[cheatTarget] - nonObstacle.ManhattanDistance(cheatTarget);

                if (numSecondsSaved < 0)
                {
                    continue;
                }

                numCheatsPerSavedSeconds[numSecondsSaved] = numCheatsPerSavedSeconds.GetValueOrDefault(numSecondsSaved) + 1;
            }

        }
        return numCheatsPerSavedSeconds;
    }

    private Dictionary<Coordinate, int> GetDistances()
    {
        var current = start;
        var count = 0;
        var foundAt = new Dictionary<Coordinate, int>();

        while (true)
        {
            foundAt[current] = count;
            count++;
            if (current == end)
            {
                return foundAt;
            }

            var next = current.GetNeighbors().Where(c => !_obstacles.Contains(c) && !foundAt.ContainsKey(c)).Single();
            current = next;
        }
    }
}