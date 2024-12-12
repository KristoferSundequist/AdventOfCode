using System.Numerics;

public class Region
{
    private readonly HashSet<Vector2> region = new();

    public Region(Vector2 start, Dictionary<Vector2, char> plots)
    {
        var current = new HashSet<Vector2> { start };

        while (current.Count > 0)
        {
            var next = new HashSet<Vector2>();

            foreach (var plot in current)
            {
                region.Add(plot);

                foreach (var surrounding in GetSurrounding(plot))
                {
                    if (
                        plots.ContainsKey(surrounding) &&
                        plots[surrounding] == plots[start] &&
                        !region.Contains(surrounding)
                    )
                    {
                        next.Add(surrounding);
                    }
                }
            }
            current = next;
        }
    }

    public bool Contains(Vector2 position) => region.Contains(position);
    public long GetArea() => region.Count;
    public long GetPerimeter() => region.Sum(p => GetSurrounding(p).Count(s => !region.Contains(s)));

    public long GetNumSides()
    {
        var top = (long)region.MinBy(p => p.Y).Y - 1;
        var left = (long)region.MinBy(p => p.X).X - 1;
        var right = (long)region.MaxBy(p => p.X).X + 1;
        var bottom = (long)region.MaxBy(p => p.Y).Y + 1;

        var numSides = 0L;
        for (var y = top; y <= bottom; y++)
        {
            numSides += GetNumHorizontalTopSides(y, left, right);
            numSides += GetNumHorizontalBottomSides(y, left, right);
        }

        for (var x = left; x <= right; x++)
        {
            numSides += GetNumVerticalLeftSides(x, top, bottom);
            numSides += GetNumVerticalRightSides(x, top, bottom);
        }

        return numSides;
    }

    private long GetNumHorizontalTopSides(long y, long startX, long endX)
    {
        var numSides = 0;
        bool sideHasStarted = false;
        for (var x = startX; x <= endX; x++)
        {
            if (sideHasStarted == false && IsTopSide(x, y))
            {
                sideHasStarted = true;
            }
            else if (sideHasStarted == true && !IsTopSide(x, y))
            {
                numSides++;
                sideHasStarted = false;
            }
        }

        return numSides;
    }

    private long GetNumHorizontalBottomSides(long y, long startX, long endX)
    {
        var numSides = 0;
        bool sideHasStarted = false;
        for (var x = startX; x <= endX; x++)
        {
            if (sideHasStarted == false && IsBottomSide(x, y))
            {
                sideHasStarted = true;
            }
            else if (sideHasStarted == true && !IsBottomSide(x, y))
            {
                numSides++;
                sideHasStarted = false;
            }
        }

        return numSides;
    }

    private long GetNumVerticalLeftSides(long x, long startY, long endY)
    {
        var numSides = 0;
        bool sideHasStarted = false;
        for (var y = startY; y <= endY; y++)
        {
            if (sideHasStarted == false && IsLeftSide(x, y))
            {
                sideHasStarted = true;
            }
            else if (sideHasStarted == true && !IsLeftSide(x, y))
            {
                numSides++;
                sideHasStarted = false;
            }
        }

        return numSides;
    }

    private long GetNumVerticalRightSides(long x, long startY, long endY)
    {
        var numSides = 0;
        bool sideHasStarted = false;
        for (var y = startY; y <= endY; y++)
        {
            if (sideHasStarted == false && IsRightSide(x, y))
            {
                sideHasStarted = true;
            }
            else if (sideHasStarted == true && !IsRightSide(x, y))
            {
                numSides++;
                sideHasStarted = false;
            }
        }

        return numSides;
    }

    private bool IsTopSide(long x, long y) => !region.Contains(new(x, y)) && region.Contains(new(x, y + 1));
    private bool IsBottomSide(long x, long y) => !region.Contains(new(x, y)) && region.Contains(new(x, y - 1));
    private bool IsLeftSide(long x, long y) => !region.Contains(new(x, y)) && region.Contains(new(x + 1, y));
    private bool IsRightSide(long x, long y) => !region.Contains(new(x, y)) && region.Contains(new(x - 1, y));

    private static Vector2[] GetSurrounding(Vector2 position)
    {
        return [
            new (position.X, position.Y - 1),
            new (position.X - 1, position.Y),
            new (position.X + 1, position.Y),
            new (position.X, position.Y + 1),
        ];
    }
}