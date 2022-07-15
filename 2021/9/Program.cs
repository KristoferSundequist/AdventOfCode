var lines = System.IO.File.ReadAllLines("data.txt");
var grid = lines.Select(line => line.ToCharArray().Select(c => Int32.Parse(c.ToString())).ToArray()).ToArray();

var lowPoints = new HashSet<Coord> { };
var sum1 = 0;
for (var y = 0; y < grid.Length; y++)
{
    for (var x = 0; x < grid[0].Length; x++)
    {
        var smallest = true;
        if (y - 1 >= 0 && grid[y - 1][x] <= grid[y][x])
        {
            smallest = false;
        }
        else if (x - 1 >= 0 && grid[y][x - 1] <= grid[y][x])
        {
            smallest = false;
        }
        if (y + 1 < grid.Length && grid[y + 1][x] <= grid[y][x])
        {
            smallest = false;
        }
        else if (x + 1 < grid[0].Length && grid[y][x + 1] <= grid[y][x])
        {
            smallest = false;
        }
        if (smallest)
        {
            sum1 += grid[y][x] + 1;
            lowPoints.Add(new Coord(y, x));
        }
    }
}

var basins = new List<Basin> { };
foreach (var lowPoint in lowPoints)
{
    basins.Add(new Basin(grid, lowPoint));
}
var top3 = basins.OrderBy(b => -b.GetSize()).Take(3).Aggregate(1, (product, b) => product*b.GetSize());
Console.WriteLine(top3);

public record Coord(int Y, int X);

public class Basin
{
    private HashSet<Coord> coords = new HashSet<Coord> { };

    public Basin(int[][] grid, Coord start)
    {
        Visit(grid, start, -1);
    }

    public int GetSize() => coords.Count;

    private void Visit(int[][] grid, Coord coord, int compareValue)
    {
        if (coords.Contains(coord))
        {
            return;
        }
        var y = coord.Y;
        var x = coord.X;
        if (y >= 0 && x >= 0 && y < grid.Length && x < grid[0].Length && grid[y][x] >= compareValue && grid[y][x] != 9)
        {
            coords.Add(coord);
            Visit(grid, coord with { X = coord.X + 1 }, grid[y][x]);
            Visit(grid, coord with { X = coord.X - 1 }, grid[y][x]);
            Visit(grid, coord with { Y = coord.Y + 1 }, grid[y][x]);
            Visit(grid, coord with { Y = coord.Y - 1 }, grid[y][x]);
        }
    }
}