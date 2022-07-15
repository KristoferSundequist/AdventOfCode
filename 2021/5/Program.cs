var verbose = Boolean.Parse(args[0]);

var data = System.IO.File.ReadAllLines("data.txt");

var lines = data
    .Select(line => line.Split(" "))
    .Select(parts => (
        parts[0].Split(",").Select(str => Int32.Parse(str)).ToArray(),
        parts[2].Split(",").Select(str => Int32.Parse(str)).ToArray())
    ).ToArray().Select(parts => new
    {
        First = new { X = parts.Item1[0], Y = parts.Item1[1] },
        Second = new { X = parts.Item2[0], Y = parts.Item2[1] }
    }).ToList();

if (verbose)
{
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(lines));
}

Part1();
Part2();

void Part1()
{
    var intersectionCount = new HashSet<object>();
    //var filteredLines = lines.Where(line => line.First.X == line.Second.X || line.First.Y == line.Second.Y).ToArray();
    var lookup = new Dictionary<object, int>();
    foreach (var line in lines)
    {
        if (line.First.X == line.Second.X)
        {
            if (line.First.Y >= line.Second.Y)
            {
                for (var i = line.Second.Y; i <= line.First.Y; i++)
                {
                    if (lookup.TryGetValue(new { X = line.First.X, Y = i }, out var val))
                    {
                        intersectionCount.Add(new { X = line.First.X, Y = i });
                        val += 1;
                    }
                    else
                    {
                        lookup.Add(new { X = line.First.X, Y = i }, 1);
                    }
                }
            }
            else
            {
                for (var i = line.First.Y; i <= line.Second.Y; i++)
                {
                    if (lookup.TryGetValue(new { X = line.First.X, Y = i }, out var val))
                    {
                        intersectionCount.Add(new { X = line.First.X, Y = i });
                        val += 1;
                    }
                    else
                    {
                        lookup.Add(new { X = line.First.X, Y = i }, 1);
                    }
                }
            }
        }
        else if (line.First.Y == line.Second.Y)
        {
            if (line.First.X >= line.Second.X)
            {
                for (var i = line.Second.X; i <= line.First.X; i++)
                {
                    if (lookup.TryGetValue(new { X = i, Y = line.First.Y }, out var val))
                    {
                        intersectionCount.Add(new { X = i, Y = line.First.Y });
                        val += 1;
                    }
                    else
                    {
                        lookup.Add(new { X = i, Y = line.First.Y }, 1);
                    }
                }
            }
            else
            {
                for (var i = line.First.X; i <= line.Second.X; i++)
                {
                    if (lookup.TryGetValue(new { X = i, Y = line.First.Y }, out var val))
                    {
                        intersectionCount.Add(new { X = i, Y = line.First.Y });
                        val += 1;
                    }
                    else
                    {
                        lookup.Add(new { X = i, Y = line.First.Y }, 1);
                    }
                }
            }
        }
    }
    Console.WriteLine(intersectionCount.Count);
}

void Part2()
{
    var intersectionCount = new HashSet<object>();
    var lookup = new Dictionary<object, int>();

    Action<object> addCoord = coord =>
    {
        if (lookup.TryGetValue(coord, out var val))
        {
            intersectionCount.Add(coord);
            val += 1;
        }
        else
        {
            lookup.Add(coord, 1);
        }
    };

    foreach (var line in lines!)
    {
        if (line.First.X == line.Second.X)
        {
            if (line.First.Y >= line.Second.Y)
            {
                for (var i = line.Second.Y; i <= line.First.Y; i++)
                {
                    addCoord(new { X = line.First.X, Y = i });
                }
            }
            else
            {
                for (var i = line.First.Y; i <= line.Second.Y; i++)
                {
                    addCoord(new { X = line.First.X, Y = i });
                }
            }
        }
        else if (line.First.Y == line.Second.Y)
        {
            if (line.First.X >= line.Second.X)
            {
                for (var i = line.Second.X; i <= line.First.X; i++)
                {
                    addCoord(new { X = i, Y = line.First.Y });
                }
            }
            else
            {
                for (var i = line.First.X; i <= line.Second.X; i++)
                {
                    addCoord(new { X = i, Y = line.First.Y });
                }
            }
        }
        else // DIAGONAL
        {
            if (line.First.X >= line.Second.X && line.First.Y >= line.Second.Y)
            {
                for (var i = 0; i <= line.First.X - line.Second.X; i++)
                {
                    addCoord(new { X = line.Second.X + i, Y = line.Second.Y + i });
                }
            }
            else if (line.First.X >= line.Second.X && line.First.Y <= line.Second.Y)
            {
                for (var i = 0; i <= line.First.X - line.Second.X; i++)
                {
                    addCoord(new { X = line.Second.X + i, Y = line.Second.Y - i });
                }
            }
            else if (line.First.X <= line.Second.X && line.First.Y <= line.Second.Y)
            {
                for (var i = 0; i <= line.Second.X - line.First.X; i++)
                {
                    addCoord(new { X = line.Second.X - i, Y = line.Second.Y - i });
                }
            }
            else
            {
                for (var i = 0; i <= line.Second.X - line.First.X; i++)
                {
                    addCoord(new { X = line.Second.X - i, Y = line.Second.Y + i });
                }
            }
        }
    }
    Console.WriteLine(intersectionCount.Count);
}


