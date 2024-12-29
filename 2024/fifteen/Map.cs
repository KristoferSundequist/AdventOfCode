public class Map
{
    Dictionary<Coordinate, char> Data = new();
    Coordinate Robot = new(0, 0);

    public Map(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                Data[new Coordinate(y, x)] = line[x];
                if (line[x] == '@')
                {
                    Robot = new Coordinate(y, x);
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

    public long GetGPSSum() => Data.Where(x => x.Value == 'O').Sum(x => x.Key.GetGpsCoordnate());

    private void Move(char move)
    {
        switch (move)
        {
            case '^':
                Move(new Coordinate(-1, 0));
                break;
            case 'v':
                Move(new Coordinate(1, 0));
                break;
            case '<':
                Move(new Coordinate(0, -1));
                break;
            case '>':
                Move(new Coordinate(0, 1));
                break;
            default:
                throw new Exception("Invalid move");
        }
    }

    private void Move(Coordinate movement)
    {
        var currentCoord = Robot + movement;
        while (true)
        {
            if (Data[currentCoord] == '#')
            {
                return;
            }
            if (Data[currentCoord] == '.')
            {
                Data[Robot] = '.';
                var newRobot = Robot + movement;
                Data[newRobot] = '@';
                Robot = newRobot;
                if (currentCoord != newRobot)
                {
                    Data[currentCoord] = 'O';
                }
                return;
            }
            currentCoord += movement;
        }
    }
}