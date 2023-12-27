using System.Collections.Immutable;

var bricks = File.ReadAllLines("data.txt").Select((str, i) => Brick.FromString(i, str)).ToList();
var fallenBricks = GetFallenBricks(bricks);

var numSafe = GetNumSafeBricks(fallenBricks);
Console.WriteLine($"Result 1: {numSafe}");

var numFallen = GetNumFallenBricks(fallenBricks);
Console.WriteLine($"Result 2: {numFallen}");


int GetNumSafeBricks(IEnumerable<Brick> bricks)
{
    var bricksById = bricks.ToImmutableDictionary(b => b.id);
    var numSafe = 0;
    foreach (var kvp in bricksById)
    {
        var bricksWithout = bricksById.Remove(kvp.Key);
        var fallenWithout = GetFallenBricks(bricksWithout.Values);
        if (fallenWithout.All(brick => bricksById[brick.id] == brick))
        {
            numSafe++;
        }
    }
    return numSafe;
}

int GetNumFallenBricks(IEnumerable<Brick> bricks)
{
    var bricksById = bricks.ToImmutableDictionary(b => b.id);
    var numFallen = 0;
    foreach (var kvp in bricksById)
    {
        var bricksWithout = bricksById.Remove(kvp.Key);
        var fallenWithout = GetFallenBricks(bricksWithout.Values);
        numFallen += fallenWithout.Count(brick => bricksById[brick.id] != brick);
    }
    return numFallen;
}


List<Brick> GetFallenBricks(IEnumerable<Brick> originalBricks)
{
    var currentBricks = originalBricks.ToList();
    while (true)
    {
        var nextBricks = new List<Brick>();
        var changed = false;
        foreach (var currentBrick in currentBricks)
        {
            var newBrick = currentBrick.Fall();
            if (newBrick == currentBrick || currentBricks.Where(other => other != currentBrick).Any(other => other.Intersects(newBrick)))
            {
                nextBricks.Add(currentBrick);
            }
            else
            {
                nextBricks.Add(newBrick);
                changed = true;
            }
        }
        if (changed == false)
        {
            return nextBricks;
        }
        else
        {
            currentBricks = nextBricks;
        }
    }
}