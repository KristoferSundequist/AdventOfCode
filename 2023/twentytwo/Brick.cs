public record struct Brick(int id, Coordinate start, Coordinate end)
{
    public static Brick FromString(int id, string str)
    {
        var parts = str.Split("~").Select(Coordinate.FromString).ToArray();
        return new Brick(id, parts[0], parts[1]);
    }

    public Brick Fall()
    {
        if (start.z == 1 || end.z == 1)
        {
            return this;
        }
        return this with
        {
            start = start with { z = start.z - 1 },
            end = end with { z = end.z - 1 }
        };
    }


    public HashSet<Coordinate> Points()
    {
        var points = new HashSet<Coordinate>();
        for (var x = start.x; x <= end.x; x++)
        {
            for (var y = start.y; y <= end.y; y++)
            {
                for (var z = start.z; z <= end.z; z++)
                {
                    points.Add(new Coordinate(x, y, z));
                }
            }
        }
        return points;
    }

    public bool Intersects(Brick other)
    {
        if (end.x < other.start.x || other.end.x < start.x)
        {
            return false;
        }

        if (end.y < other.start.y || other.end.y < start.y)
        {
            return false;
        }

        if (end.z < other.start.z || other.end.z < start.z)
        {
            return false;
        }

        return true;
    }
}

public record struct Coordinate(int x, int y, int z)
{
    public static Coordinate FromString(string str)
    {
        var values = str.Split(",").Select(int.Parse).ToArray();
        return new Coordinate(values[0], values[1], values[2]);
    }
}