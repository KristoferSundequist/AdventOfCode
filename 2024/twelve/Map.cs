using System.Numerics;

class Map
{
    private Dictionary<Vector2, char> plots = new();
    private List<Region> regions = new();

    public Map(char[][] input)
    {
        // Built plots
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                plots[new(x, y)] = input[y][x];
            }
        }

        // Built regions
        foreach (var plot in plots)
        {
            if (regions.Any(r => r.Contains(plot.Key)))
            {
                continue;
            }

            regions.Add(new Region(plot.Key, plots));
        }
    }

    public long GetTotalFencePrice1() => regions.Sum(r => r.GetArea() * r.GetPerimeter());
    public long GetTotalFencePrice2() => regions.Sum(r => r.GetArea() * r.GetNumSides());
}