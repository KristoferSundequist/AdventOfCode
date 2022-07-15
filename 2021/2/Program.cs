Part2();

void Part2()
{
    var lines = System.IO.File.ReadAllLines("data.txt")
    ?.Select(l => l.Split(" "))
    ?.Select(arr => (arr[0], Int32.Parse(arr[1])));

    if (lines is null)
    {
        throw new Exception("lines is null");
    }

    var horizontal = 0;
    var depth = 0;
    var aim = 0;

    foreach (var (direction, amount) in lines)
    {
        switch (direction)
        {
            case "forward":
                horizontal += amount;
                depth += aim * amount;
                break;
            case "down":
                aim += amount;
                break;
            case "up":
                aim -= amount;
                break;
            default:
                throw new Exception("Unexpected direction");
        };
    }

    Console.WriteLine(horizontal * depth);
}

void Part1()
{
    var lines = System.IO.File.ReadAllLines("data.txt")
    ?.Select(l => l.Split(" "))
    ?.Select(arr => (arr[0], Int32.Parse(arr[1])));

    if (lines is null)
    {
        throw new Exception("lines is null");
    }

    var horizontal = 0;
    var depth = 0;

    foreach (var (direction, amount) in lines)
    {
        var _ = direction switch
        {
            "forward" => horizontal += amount,
            "down" => depth += amount,
            "up" => depth -= amount,
            _ => throw new Exception("Unexpected direction")
        };
    }

    Console.WriteLine(horizontal * depth);
}


