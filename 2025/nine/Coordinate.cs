public record struct Vec2(long X, long Y)
{
    public static Vec2 FromString(string s)
    {
        var parts = s.Split(',');
        return new Vec2(long.Parse(parts[0]), long.Parse(parts[1]));
    }
}