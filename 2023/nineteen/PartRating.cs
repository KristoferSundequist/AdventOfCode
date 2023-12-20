public record struct PartRating(int x, int m, int a, int s)
{
    public static PartRating FromString(string str)
    {
        var parts = str[1..^1].Split(",");
        var numbers = parts.Select(str => str.Split("=")[1]).ToArray();
        return new PartRating(int.Parse(numbers[0]), int.Parse(numbers[1]), int.Parse(numbers[2]), int.Parse(numbers[3]));
    }

    public long Sum() => x + m + a + s;
}