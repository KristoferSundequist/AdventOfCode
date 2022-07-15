var lines = System.IO.File.ReadAllLines("data.txt");
int[][] grid = lines.Select(line => line.ToCharArray().Select(c => Int32.Parse(c.ToString())).ToArray()).ToArray();

var biggrid = GetBigGrid(grid);

var gridToUse = biggrid;

int maxY = gridToUse.Length - 1;
int maxX = gridToUse[0].Length - 1;

//var visited = new HashSet<Point> { new Point(0, 0) };
var paths = new List<HashSet<Point>> { };
var shortest = long.MaxValue;

var allShortest = new Dictionary<Point, long> { };

//Visit(new HashSet<Point> { }, new Point(0, 0), 0);
Visit2(new Point(0, 0), 0);

Console.WriteLine(shortest);

void Visit2(Point current, long sum)
{
    if (allShortest.TryGetValue(current, out var pastSum))
    {
        if (pastSum <= sum)
        {
            return;
        }
        else
        {
            allShortest[current] = sum;
        }
    }
    else
    {
        allShortest.Add(current, sum);
    }
    //Console.WriteLine($"{y} {x}");
    if (current.y == maxY && current.x == maxX && sum < shortest)
    {
        shortest = sum;
        return;
    }
    if (sum > shortest)
    {
        return;
    }
    var next =
        new HashSet<Point?> { Get(current.y + 1, current.x), Get(current.y, current.x + 1), Get(current.y - 1, current.x), Get(current.y, current.x - 1) }
        .Where(p => p is not null)
        .ToHashSet();

    foreach (var p in next)
    {
        if (p is not null)
        {
            Visit2(new Point(p.y, p.x), sum + gridToUse[p.y][p.x]);
        }
    }
}

Point? Get(int y, int x)
{
    if (0 <= y && y <= maxY && 0 <= x && x <= maxX)
    {
        return new Point(y, x);
    }
    else
    {
        return null;
    }
}

int[][] GetBigGrid(int[][] originalGrid)
{
    var row = new List<int[][]> { originalGrid };
    for (var i = 1; i < 5; i++)
    {
        row.Add(IncrementGrid(row.Last()));
    }
    var firstRow = MergeAlongX(row);

    var rows = new List<int[][]> { firstRow };
    for (var i = 1; i < 5; i++)
    {
        rows.Add(IncrementGrid(rows.Last()));
    }
    var full = MergeAlongY(rows);
    return full;
}

int[][] MergeAlongX(List<int[][]> grids)
{
    var gridHeight = grids[0].Length;
    var gridWidth = grids[0][0].Length;
    int[][] newGrid = Enumerable.Range(0, gridHeight).Select(row => Enumerable.Range(0, gridWidth * grids.Count).ToArray()!).ToArray()!;
    for (var gi = 0; gi < grids.Count; gi++)
    {
        for (var y = 0; y < gridHeight; y++)
        {
            for (var x = 0; x < gridWidth; x++)
            {
                newGrid[y][gi * gridWidth + x] = grids[gi][y][x];
            }
        }
    }
    return newGrid;
}

int[][] MergeAlongY(List<int[][]> grids)
{
    var gridHeight = grids[0].Length;
    var gridWidth = grids[0][0].Length;
    int[][] newGrid = Enumerable.Range(0, gridHeight * grids.Count).Select(row => Enumerable.Range(0, gridWidth).ToArray()!).ToArray()!;
    for (var gi = 0; gi < grids.Count; gi++)
    {
        for (var y = 0; y < gridHeight; y++)
        {
            for (var x = 0; x < gridWidth; x++)
            {
                newGrid[gi * gridHeight + y][x] = grids[gi][y][x];
            }
        }
    }
    return newGrid;
}

int[][] IncrementGrid(int[][] originalGrid)
{
    var newGrid = DeepCopy(originalGrid);
    for (var y = 0; y < newGrid.Length; y++)
    {
        for (var x = 0; x < newGrid[y].Length; x++)
        {
            newGrid[y][x] = (newGrid[y][x] % 9) + 1;
        }
    }
    return newGrid;
}

T DeepCopy<T>(T obj) => System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(obj)!)!;

record Point(int y, int x);