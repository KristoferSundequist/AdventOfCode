using System.Configuration.Assemblies;
using System.Numerics;

public class LockOrKey
{
    private HashSet<Vector2> Schematic { get; set; } = new();

    public LockOrKey(string[] input)
    {
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '#')
                {
                    Schematic.Add(new Vector2(x, y));
                }
            }
        }
    }

    public bool Fits(LockOrKey other)
    {
        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (Schematic.Contains(new Vector2(x, y)) && other.Schematic.Contains(new Vector2(x, y)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Visualize()
    {
        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Console.Write(Schematic.Contains(new Vector2(x, y)) ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    public bool IsLock()
    {
        for (var i = 0; i < 5; i++)
        {
            if (!Schematic.Contains(new Vector2(i, 0)))
            {
                return false;
            }
        }
        return true;
    }
}