public record struct Line
{
    public Vec2 Start { get; }
    public Vec2 End { get; }

    public Line(Vec2 start, Vec2 end)
    {
        Start = start;
        End = end;
    }

    public bool Crosses(Line other)
    {

        if (Start.X == End.X && other.Start.Y == other.End.Y)
        {
            var withinX = Math.Min(other.Start.X, other.End.X) <= Start.X && Start.X <= Math.Max(other.Start.X, other.End.X);
            var withinY = Math.Min(Start.Y, End.Y) < other.Start.Y && other.Start.Y < Math.Max(Start.Y, End.Y);
            return withinX && withinY;
        }
        else if (Start.Y == End.Y && other.Start.X == other.End.X)
        {
            var withinY = Math.Min(other.Start.Y, other.End.Y) <= Start.Y && Start.Y <= Math.Max(other.Start.Y, other.End.Y);
            var withinX = Math.Min(Start.X, End.X) <= other.Start.X && other.Start.X <= Math.Max(Start.X, End.X);
            return withinX && withinY;
        }
        else
        {
            // parallel lines can't cross
            return false;
        }
    }

}