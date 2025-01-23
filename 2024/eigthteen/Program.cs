var lines = File.ReadAllLines("input.txt");

var memorySpace = new MemorySpace(lines[..1024], 70, 70);
Console.WriteLine(memorySpace.GetShortestPath());

for (var i = 1025; i < lines.Length; i++)
{
    var memorySpace2 = new MemorySpace(lines[..i], 70, 70);
    var maybeShortesPath = memorySpace2.GetShortestPath();
    if (maybeShortesPath is null)
    {
        Console.WriteLine(lines[i-1]);
        break;
    }
}



