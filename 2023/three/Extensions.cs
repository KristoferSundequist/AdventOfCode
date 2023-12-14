public static class MyExtensions
{
    public static void SetIn<T>(this Dictionary<int, Dictionary<int, T>> dict, int y, int x, T thing)
    {
        if (dict.TryGetValue(y, out var line))
        {
            if (line.ContainsKey(x))
            {
                throw new Exception($"x: {x} already exists for y: {y}");
            }
            line.Add(x, thing);
        }
        else
        {
            dict.Add(y, new Dictionary<int, T> { { x, thing } });
        }
    }

    public static bool ContainsIn<T>(this Dictionary<int, Dictionary<int, T>> dict, int y, int x)
    {
        if (dict.TryGetValue(y, out var line))
        {
            if (line.ContainsKey(x))
            {
                return true;
            }
        }
        return false;
    }
}