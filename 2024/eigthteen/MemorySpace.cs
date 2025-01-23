public class MemorySpace
{
    private HashSet<Coordinate> _coordinates = new HashSet<Coordinate>();
    private long _width;
    private long _height;

    public MemorySpace(string[] input, long width, long height)
    {
        foreach (var line in input)
        {
            _coordinates.Add(Coordinate.FromString(line));
        }
        _width = width;
        _height = height;
    }

    public void Draw()
    {
        for (var y = 0; y <= _height; y++)
        {
            for (var x = 0; x <= _width; x++)
            {
                Console.Write(_coordinates.Contains(new Coordinate(x, y)) ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    public long? GetShortestPath()
    {
        var start = new Coordinate(0, 0);

        var found = new Dictionary<Coordinate, long>
        {
            { start, 0 }
        };

        var frontier = new Dictionary<Coordinate, long>
        {
            { start, 0 }
        };

        while (frontier.Count > 0)
        {
            var newFrontier = new Dictionary<Coordinate, long>();

            foreach (var coord in frontier.Keys)
            {
                var next = GetNext(coord);

                foreach (var n in next)
                {
                    if (!found.ContainsKey(n))
                    {
                        found[n] = frontier[coord] + 1;
                        newFrontier[n] = frontier[coord] + 1;
                    }
                }
            }

            frontier = newFrontier;
        }

        return found.TryGetValue(new Coordinate(_width , _height), out var result) ? result : null;
    }

    private HashSet<Coordinate> GetNext(Coordinate coord)
    {
        List<Coordinate> candidates = [
            coord.Up(),
            coord.Down(),
            coord.Left(),
            coord.Right()
        ];

        return candidates.Where(c => c.X >= 0 && c.X <= _width && c.Y >= 0 && c.Y <= _height && !_coordinates.Contains(c)).ToHashSet();
    }
}