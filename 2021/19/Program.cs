
var lines = System.IO.File.ReadAllLines("data.txt");
var scanners = new List<List<Point>> { };

var currentScanner = new List<Point> { };
foreach (var line in lines[1..])
{
    if (line.Length < 3)
    {
        scanners.Add(currentScanner);
        continue;
    }
    if (line.StartsWith("---"))
    {
        currentScanner = new List<Point> { };
        continue;
    }
    var parsedLine = line.Split(",").Select(str => Int32.Parse(str)).ToArray();
    currentScanner.Add(new Point(parsedLine[0], parsedLine[1], parsedLine[2]));
}

var alignedScanners = new List<Scanner> { new Scanner(scanners[0], new Point(0, 0, 0), 0) };
var scanned = scanners.Select(_ => false).ToList();

while (alignedScanners.Count < scanners.Count)
{
Start:
    foreach (var s in alignedScanners)
    {
        if (scanned[s.id]) continue;
        for (var i = 0; i < scanners.Count; i++)
        {
            if (alignedScanners.Any(s => s.id == i))
            {
                continue;
            }
            var maybeAlignedScanner = GetAligned(s, scanners[i], i);
            if (maybeAlignedScanner is not null)
            {
                alignedScanners.Add(maybeAlignedScanner);
                goto Start;
            }
        }
        scanned[s.id] = true;
    }
}

var allPoints = new HashSet<Point> { };
foreach (var s in alignedScanners)
{
    foreach (var p in s.points)
    {
        allPoints.Add(p.Add(s.offset));
    }
}
Console.WriteLine("TOTAL COUNT: " + allPoints.Count);

var curMax = 0;
foreach (var s in alignedScanners)
{
    foreach (var s2 in alignedScanners)
    {
        curMax = Math.Max(curMax, s.offset.ManhattanDistance(s2.offset));
    }
}

Console.WriteLine($"Largest manhattan: {curMax}");

Scanner? GetAligned(Scanner scannerA, List<Point> scannerB, int id)
{
    foreach (var orientation in Enumerable.Range(0, 24))
    {
        var scannarBOriented = scannerB.Select(p => Orient(p, orientation)).ToList();
        var tally = new Dictionary<Point, int> { };
        foreach (var pa in scannerA.points)
        {
            foreach (var pb in scannarBOriented)
            {
                var offset = pa.Subtract(pb);
                if (tally.TryGetValue(offset, out var count))
                {
                    tally[offset] = count + 1;
                    if (count + 1 >= 12)
                    {
                        return new Scanner(scannarBOriented, scannerA.offset.Add(offset), id);
                    }
                }
                else
                {
                    tally[offset] = 1;
                }
            }
        }
    }
    return null;
}


Point Orient(Point p, int orientation)
{
    return (orientation + 1) switch
    {
        1 => new Point(p.x, p.y, p.z),
        2 => new Point(p.x, -p.z, p.y),
        3 => new Point(p.x, -p.y, -p.z),
        4 => new Point(p.x, p.z, -p.y),

        5 => new Point(-p.x, -p.y, p.z),
        6 => new Point(-p.x, -p.z, -p.y),
        7 => new Point(-p.x, p.y, -p.z),
        8 => new Point(-p.x, p.z, p.y),

        9 => new Point(p.y, -p.x, p.z),
        10 => new Point(p.y, -p.z, -p.x),
        11 => new Point(p.y, p.x, -p.z),
        12 => new Point(p.y, p.z, p.x),

        13 => new Point(-p.y, p.x, p.z),
        14 => new Point(-p.y, -p.z, p.x),
        15 => new Point(-p.y, -p.x, -p.z),
        16 => new Point(-p.y, p.z, -p.x),

        17 => new Point(p.z, p.y, -p.x),
        18 => new Point(p.z, p.x, p.y),
        19 => new Point(p.z, -p.y, p.x),
        20 => new Point(p.z, -p.x, -p.y),

        21 => new Point(-p.z, p.y, p.x),
        22 => new Point(-p.z, -p.x, p.y),
        23 => new Point(-p.z, -p.y, -p.x),
        24 => new Point(-p.z, p.x, -p.y),

        _ => throw new Exception("Unexpected orientation")
    };
}


public record Scanner(List<Point> points, Point offset, int id);

public record Point(int x, int y, int z)
{
    public Point RotateAroundX() => new Point(x, -z, y);
    public Point RotateAroundY() => new Point(x, z, -y);
    public Point RotateAroundZ() => new Point(y, -x, z);
    public Point Subtract(Point other) => new Point(x - other.x, y - other.y, z - other.z);
    public Point Add(Point other) => new Point(x + other.x, y + other.y, z + other.z);
    public int ManhattanDistance(Point other) => x - other.x + y - other.y + z - other.z;
}