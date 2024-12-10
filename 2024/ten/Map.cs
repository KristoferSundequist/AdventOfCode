using System.Numerics;

class Map
{
    private Dictionary<Vector2, int> _map = new Dictionary<Vector2, int>();

    public Map(char[][] input)
    {
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                _map[new Vector2(x, y)] = int.Parse(input[y][x].ToString());
            }
        }
    }

    public int GetAllTrailHeadScores(bool distinct)
    {
        return _map
            .Where(p => p.Value == 0)
            .Select(p => GetTrailHeadScore(p.Key, distinct))
            .Sum();
    }

    private int GetTrailHeadScore(Vector2 trailHead, bool distinct)
    {
        var nines = 0;

        var current = GetUphillSurrounding(trailHead).ToList();
        while (current.Count > 0)
        {
            var next = new List<Vector2>();
            foreach (var currentPosition in current)
            {
                if (_map[currentPosition] == 9)
                {
                    nines++;
                }
                var nextSurrounding = GetUphillSurrounding(currentPosition);
                next.AddRange(nextSurrounding);
            }
            current = !distinct ? next.Distinct().ToList() : next;
        }
        return nines;
    }

    private HashSet<Vector2> GetUphillSurrounding(Vector2 position)
    {
        var surrounding = new HashSet<Vector2> {
            new Vector2(position.X - 1, position.Y),
            new Vector2(position.X + 1, position.Y),
            new Vector2(position.X, position.Y - 1),
            new Vector2(position.X, position.Y + 1)
        };

        return surrounding
            .Where(p => _map.ContainsKey(p))
            .Where(p => _map[p] == _map[position] + 1)
            .ToHashSet();
    }
}