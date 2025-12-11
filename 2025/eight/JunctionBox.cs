public record JunctionBox(long X, long Y, long Z)
{
    public static JunctionBox FromString(string s)
    {
        var parts = s.Split(',').Select(long.Parse).ToArray();
        return new JunctionBox(parts[0], parts[1], parts[2]);
    }

    public double Distance(JunctionBox other)
    {
        var distance = Math.Sqrt(
            Math.Pow(other.X - X, 2) +
            Math.Pow(other.Y - Y, 2) +
            Math.Pow(other.Z - Z, 2)
        );

        // detec overflow
        if (double.IsInfinity(distance))
        {
            throw new OverflowException("Distance calculation overflowed");
        }
        if (distance < 0)
        {
            throw new Exception("Distance calculation resulted in negative value");
        }

        return distance;
    }
}