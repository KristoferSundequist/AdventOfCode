var lines = File.ReadAllLines("input.txt");
var robots = lines.Select(Robot.Parse).ToList();

//var bottomRightCorner = new DiscreteVector2(11, 7);
var bottomRightCorner = new DiscreteVector2(101, 103);
var robotsAfter100 = GetRobotsAfterNTicks(robots, bottomRightCorner, 100000);
var result1 = robotsAfter100.Select(r => r.GetQuadrant(bottomRightCorner)).Where(q => q.HasValue).GroupBy(q => (int)q!).Select(g => g.Count()).Aggregate(1, (a, b) => a * b);
Console.WriteLine($"Part 1: {result1}");


List<Robot> GetRobotsAfterNTicks(List<Robot> robots, DiscreteVector2 bottomRightCorner, int n)
{
    var newRobots = new List<Robot>();
    for (var i = 0; i < n; i++)
    {
        newRobots = robots.Select(r => r.Tick(bottomRightCorner)).ToList();
        robots = newRobots;
        if (HasChrismasTree(robots))
        {
            Console.WriteLine($"Found a chrismas tree at tick {i + 1}");
            break;
        }
    }

    return newRobots;
}

bool HasChrismasTree(List<Robot> robots)
{
    // if the robots are aligned such that they are more then 12 in a row anywhere, then we have a chrismas tree
    var robotsByY = robots.GroupBy(r => r.Position.Y).OrderBy(g => g.Key).ToList();
    foreach (var group in robotsByY)
    {
        var orderedRobots = group.OrderBy(r => r.Position.X).ToList();
        var count = 1;
        for (var i = 1; i < orderedRobots.Count; i++)
        {
            if (orderedRobots[i].Position.X - orderedRobots[i - 1].Position.X == 1)
            {
                count++;
            }
            else
            {
                count = 1;
            }

            if (count >= 12)
            {
                return true;
            }
        }
    }
    return false;
}

void DrawRobots(List<Robot> robots, DiscreteVector2 bottomRightCorner)
{
    var grid = new char[bottomRightCorner.Y, bottomRightCorner.X];
    for (var y = 0; y < grid.GetLength(0); y++)
    {
        for (var x = 0; x < grid.GetLength(1); x++)
        {
            grid[y, x] = '.';
        }
    }

    foreach (var robot in robots)
    {
        grid[robot.Position.Y, robot.Position.X] = '#';
    }

    for (var y = 0; y < grid.GetLength(0); y++)
    {
        for (var x = 0; x < grid.GetLength(1); x++)
        {
            Console.Write(grid[y, x]);
        }

        Console.WriteLine();
    }
}