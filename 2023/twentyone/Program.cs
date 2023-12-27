var lines = File.ReadAllLines("./data.txt");
var gardenPlots = new HashSet<Coordinate>();
var rocks = new HashSet<Coordinate>();
var startingCoord = new Coordinate(-1, -1);
var size = lines.Length;
for (var y = 0; y < size; y++)
{
    for (var x = 0; x < size; x++)
    {
        var coord = new Coordinate(y, x);
        var thing = lines[y][x];
        if (thing == '.')
        {
            gardenPlots.Add(coord);
        }
        else if (thing == '#')
        {
            rocks.Add(coord);
        }
        else if (thing == 'S')
        {
            gardenPlots.Add(coord);
            startingCoord = coord;
        }
        else
        {
            throw new Exception($"unko0wn thing {thing}");
        }

    }
}

// part 1
var currentCoords = new HashSet<Coordinate> { startingCoord };
var nextCoords = new HashSet<Coordinate>();
for (var i = 0; i < 64; i++)
{
    foreach (var coord in currentCoords)
    {
        nextCoords = nextCoords.Union(coord.GetAdjacentCoordinates(gardenPlots)).ToHashSet();
    }
    currentCoords = nextCoords;
    nextCoords = new HashSet<Coordinate>();
}
Console.WriteLine($"part 1: {currentCoords.Count}");

// part2

// logged differences and differences of differences and spotted a pattern of size 131 (input size)
// then could just calculate the size without actually simulating it

long current = 33494; // 196 (65 + 131) (65 from 26501365 mod 131)
long diff = 59317; // 327v - 196v
for (long i = 196; i < 26501365; i += 131)
{
    current += diff;
    diff += 29578; // constant acceleration of size
}
Console.WriteLine($"part 2: {current}");

// var currentCoords2 = new HashSet<Coordinate> { startingCoord };
// var nextCoords2 = new HashSet<Coordinate>();
// var sizes = new Dictionary<int, int>();
// for (var i = 0; i < 1000; i++)
// {
//     sizes[i] = currentCoords2.Count;
//     Console.WriteLine($"{i}: {sizes[i]}");

//     foreach (var coord in currentCoords2)
//     {
//         nextCoords2 = nextCoords2.Union(coord.GetAdjacentInfiniteCoordinates(gardenPlots, size)).ToHashSet();
//     }
//     currentCoords2 = nextCoords2;
//     nextCoords2 = new HashSet<Coordinate>();
// }
// foreach (var kvp in sizes)
// {
//     Console.WriteLine($"{kvp.Key} - {kvp.Value}");
// }

record struct Coordinate(int y, int x)
{
    public HashSet<Coordinate> GetAdjacentCoordinates(HashSet<Coordinate> gardenPlots)
    {
        return new Coordinate[] {
            this with { x = this.x, y = this.y - 1 },
            this with { x = this.x - 1, y = this.y },
            this with { x = this.x + 1, y = this.y },
            this with { x = this.x, y = this.y + 1 },
        }.Where(gardenPlots.Contains).ToHashSet();
    }

    private int TranslateNumber(int v, int size)
    {
        var result = v % size;
        if (result < 0)
        {
            return size + result;
        }
        return result;
    }

    public Coordinate Translate(int size) => new Coordinate(TranslateNumber(y, size), TranslateNumber(x, size));

    public HashSet<Coordinate> GetAdjacentInfiniteCoordinates(HashSet<Coordinate> gardenPlots, int size)
    {
        return new Coordinate[] {
            this with { x = this.x, y = this.y - 1 },
            this with { x = this.x - 1, y = this.y },
            this with { x = this.x + 1, y = this.y },
            this with { x = this.x, y = this.y + 1 },
        }.Where(coord => gardenPlots.Contains(coord.Translate(size))).ToHashSet();
    }
}

