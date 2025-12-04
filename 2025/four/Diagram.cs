public class Diagram
{
    private HashSet<Position> rollsOfPaper = new();

    public Diagram(string[] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[0].Length; x++)
            {
                if (lines[y][x] == '@')
                {
                    rollsOfPaper.Add(new Position { Y = y, X = x });
                }
            }
        }
    }

    public HashSet<Position> GetAccessibleRollsOfPaper()
    {
        return rollsOfPaper.Where(rollOfPaper => rollOfPaper.GetAdjacentPositions().Count(rollsOfPaper.Contains) < 4).ToHashSet();
    }

    public int RemoveAllPaperThatYouCan()
    {
        var removeCount = 0;

        do
        {
            var accessibleRollsOfPaper = GetAccessibleRollsOfPaper();
            removeCount += accessibleRollsOfPaper.Count;

            foreach (var rollOfPaper in accessibleRollsOfPaper)
            {
                rollsOfPaper.Remove(rollOfPaper);
            }
        } while (GetAccessibleRollsOfPaper().Count > 0);

        return removeCount;
    }

    public void Visualize()
    {
        for (int y = 0; y < rollsOfPaper.Max(p => p.Y); y++)
        {
            for (int x = 0; x < rollsOfPaper.Max(p => p.X); x++)
            {
                if (rollsOfPaper.Contains(new Position(y, x)))
                {
                    Console.Write("@");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
    }
}

public record struct Position(int Y, int X)
{
    public HashSet<Position> GetAdjacentPositions()
    {
        return new HashSet<Position>
        {
            new Position(Y - 1, X - 1),
            new Position(Y - 1, X),
            new Position(Y - 1, X + 1),
            new Position(Y, X - 1),
            new Position(Y, X + 1),
            new Position(Y + 1, X - 1),
            new Position(Y + 1, X),
            new Position(Y + 1, X + 1),
        };
    }
}