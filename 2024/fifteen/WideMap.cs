public class WideMap
{
    Dictionary<Coordinate, char> Data = new();
    Coordinate Robot = new(0, 0);

    public WideMap(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                var coord = new Coordinate(y, x * 2);
                var rightCoord = coord.MoveRight();
                if (line[x] == 'O')
                {
                    Data[coord] = '[';
                    Data[rightCoord] = ']';
                }
                else if (line[x] == '@')
                {
                    Robot = coord;
                    Data[coord] = '.';
                    Data[rightCoord] = '.';
                }
                else if (line[x] == '#')
                {
                    Data[coord] = '#';
                    Data[rightCoord] = '#';
                }
                else if (line[x] == '.')
                {
                    Data[coord] = '.';
                    Data[rightCoord] = '.';
                }
                else
                {
                    throw new Exception($"Invalid character {line[x]}");
                }
            }
        }

        if (Robot == new Coordinate(0, 0))
        {
            throw new Exception("Robot not found");
        }
    }

    public void MakeMoves(string moves)
    {
        foreach (var move in moves)
        {
            Move(move);
        }
    }

    public void Draw()
    {
        var minX = Data.Keys.Min(x => x.X);
        var maxX = Data.Keys.Max(x => x.X);
        var minY = Data.Keys.Min(x => x.Y);
        var maxY = Data.Keys.Max(x => x.Y);

        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var coord = new Coordinate(y, x);
                if (Robot == coord)
                {
                    Console.Write('@');
                }
                else if (Data.ContainsKey(coord))
                {
                    Console.Write(Data[coord]);
                }
                else
                {
                    throw new Exception("Invalid coordinate");
                }
            }
            Console.WriteLine();
        }
    }

    public long GetGPSSum() => Data.Where(x => x.Value == '[').Sum(x => x.Key.GetGpsCoordnate());

    private void Move(char move)
    {
        switch (move)
        {
            case '^':
                Move(Coordinate.Up());
                break;
            case 'v':
                Move(Coordinate.Down());
                break;
            case '<':
                Move(Coordinate.Left());
                break;
            case '>':
                Move(Coordinate.Right());
                break;
            default:
                throw new Exception($"Invalid move {move}");
        }
    }

    private void Move(Coordinate movement)
    {
        if (Data[Robot + movement] == '.')
        {
            Robot = Robot + movement;
            return;
        }

        if (Data[Robot + movement] == '#')
        {
            return;
        }

        var boxes = movement.IsHorizontal()
            ? FindAllHorizontalBoxesToMove(movement)
            : FindAllVerticalBoxesToMove(movement);

        if (boxes.Count == 0)
        {
            return;
        }

        foreach (var box in boxes)
        {
            Data[box.Key] = '.';
        }
        foreach (var box in boxes)
        {
            Data[box.Key + movement] = box.Value;
        }

        Robot = Robot + movement;
    }

    private Dictionary<Coordinate, char> FindAllHorizontalBoxesToMove(Coordinate movement)
    {
        var currentCoord = Robot + movement;
        var boxes = new Dictionary<Coordinate, char>();

        while (true)
        {
            var currentCoordValue = Data[currentCoord];

            if (currentCoordValue == '[' || currentCoordValue == ']')
            {
                boxes[currentCoord] = currentCoordValue;
            }
            else if (currentCoordValue == '.')
            {
                return boxes;
            }
            else if (currentCoordValue == '#')
            {
                return [];
            }
            else
            {
                throw new Exception($"Invalid character {currentCoordValue}");
            }
            currentCoord += movement;
        }
    }

    private Dictionary<Coordinate, char> FindAllVerticalBoxesToMove(Coordinate movement)
    {
        var boxes = new Dictionary<Coordinate, char>();
        var frontier = new HashSet<Coordinate> { Robot + movement };

        while (true)
        {
            var expandedFrontier = ExpandFrontierHorizontally(frontier);
            var newBoxes = expandedFrontier.ToDictionary(x => x, x => Data[x]);
            boxes = boxes.Union(newBoxes).ToDictionary();
            var newFrontier = expandedFrontier.Select(x => x + movement).ToHashSet();
            if (newFrontier.Any(coord => Data[coord] == '#'))
            {
                return [];
            }
            if (newFrontier.All(coord => Data[coord] == '.'))
            {
                return boxes;
            }
            frontier = newFrontier.Where(coord => Data[coord] == ']' || Data[coord] == '[').ToHashSet();
        }
    }

    private HashSet<Coordinate> ExpandFrontierHorizontally(HashSet<Coordinate> frontier)
    {
        var newFrontier = new HashSet<Coordinate>(frontier);
        foreach (var currentCoord in frontier)
        {
            var currentValue = Data[currentCoord];
            if (currentValue == '[')
            {
                var rightCoord = currentCoord.MoveRight();
                if (Data[rightCoord] != ']')
                {
                    throw new Exception("Expected ]");
                }
                newFrontier.Add(rightCoord);
            }
            else if (currentValue == ']')
            {
                var leftCoord = currentCoord.MoveLeft();
                if (Data[leftCoord] != '[')
                {
                    throw new Exception("Expected [");
                }
                newFrontier.Add(leftCoord);
            }
        }
        return newFrontier;
    }
}