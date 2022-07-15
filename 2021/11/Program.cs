var lines = System.IO.File.ReadAllLines("data.txt");

var grid = lines.Select(line => line.ToCharArray().Select(c => Int32.Parse(c.ToString())).ToList()).ToList();

var flashes = 0;
foreach (var i in Enumerable.Range(1, 300))
{
    (var newFlashes, grid, var allFlashed) = Advance(grid);
    if (allFlashed)
    {
        Console.WriteLine($"All flashed at iteration: {i}");
    }
    flashes += newFlashes;
}
Console.WriteLine(flashes);

(int, List<List<int>>, bool) Advance(List<List<int>> grid)
{
    var newGrid = new List<List<int>> { };
    var hasFlashed = new List<List<bool>> { };
    foreach (var row in grid)
    {
        var newRow = new List<int> { };
        var newFlashedRow = new List<bool> { };
        foreach (var cell in row)
        {
            newRow.Add(cell + 1);
            newFlashedRow.Add(false);
        }
        newGrid.Add(newRow);
        hasFlashed.Add(newFlashedRow);
    }
    var flashed = false;
    var flashes = 0;

    do
    {
        flashed = false;
        for (var y = 0; y < newGrid.Count; y++)
        {
            for (var x = 0; x < newGrid[0].Count; x++)
            {
                if (newGrid[y][x] > 9 && !hasFlashed[y][x])
                {
                    hasFlashed[y][x] = true;
                    flashed = true;
                    flashes += 1;
                    Increase(x - 1, y - 1, newGrid);
                    Increase(x - 1, y, newGrid);
                    Increase(x - 1, y + 1, newGrid);
                    Increase(x, y - 1, newGrid);
                    Increase(x, y + 1, newGrid);
                    Increase(x + 1, y - 1, newGrid);
                    Increase(x + 1, y, newGrid);
                    Increase(x + 1, y + 1, newGrid);
                }
            }
        }
    } while (flashed == true);

    for (var y = 0; y < newGrid.Count; y++)
    {
        for (var x = 0; x < newGrid[0].Count; x++)
        {
            if (hasFlashed[y][x])
            {
                newGrid[y][x] = 0;
            }
        }
    }

    return (flashes, newGrid, hasFlashed.All(row => row.All(cell => cell)));
}

void Increase(int x, int y, List<List<int>> grid)
{
    try
    {
        grid[y][x] += 1;
    }
    catch (Exception _)
    {
    }
}

void PrintGrid(List<List<int>> grid)
{
    Console.WriteLine("----------------------");
    foreach (var row in grid)
    {
        foreach (var cell in row)
        {
            Console.Write(cell);
        }
        Console.WriteLine("");
    }
    Console.WriteLine("----------------------");
}