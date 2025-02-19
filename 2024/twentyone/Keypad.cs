public class Keypad(Dictionary<Coordinate, char> CoordToChar)
{
    public readonly Dictionary<char, Coordinate> CharToCoord = CoordToChar.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    public string GetPath(char source, char target)
    {
        if (source == target)
        {
            return "A";
        }
        if (IsNumerical(source) || IsNumerical(target))
        {
            return GetNumericalPath(source, target);
        }

        if (IsDirectional(source) || IsDirectional(target))
        {
            return GetDirectionalPath(source, target);
        }

        throw new ArgumentException($"Didnt expect source {source} and target {target}");
    }

    private bool IsDirectional(char c) => c == '^' || c == 'v' || c == '<' || c == '>';
    private bool IsNumerical(char c) => c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9';

    private string GetNumericalPath(char source, char target)
    {
        var diff = CharToCoord[target] - CharToCoord[source];

        var horizontal = new string(diff.X < 0 ? '<' : '>', Math.Abs(diff.X));
        var vertical = new string(diff.Y < 0 ? '^' : 'v', Math.Abs(diff.Y));

        var path = diff.X < 0 ? $"{horizontal}{vertical}" : $"{vertical}{horizontal}";

        if (PathIsValid(source, path))
        {
            return $"{path}A";
        }
        return diff.X < 0 ? $"{vertical}{horizontal}A" : $"{horizontal}{vertical}A";
    }

    private string GetDirectionalPath(char source, char target)
    {
        var diff = CharToCoord[target] - CharToCoord[source];

        var horizontal = new string(diff.X < 0 ? '<' : '>', Math.Abs(diff.X));
        var vertical = new string(diff.Y < 0 ? '^' : 'v', Math.Abs(diff.Y));

        var isHorizontalFirst = (diff.X, diff.Y) switch
        {
            ( < 0, > 0) => true,
            ( > 0, < 0) => false,
            ( < 0, < 0) => true,
            ( > 0, > 0) => false,
            _ => false
        };

        var path = isHorizontalFirst ? $"{horizontal}{vertical}" : $"{vertical}{horizontal}";

        if (PathIsValid(source, path))
        {
            return $"{path}A";
        }
        return isHorizontalFirst ? $"{vertical}{horizontal}A" : $"{horizontal}{vertical}A";
    }

    private bool PathIsValid(char source, string path)
    {
        var current = CharToCoord[source];
        foreach (var direction in path)
        {
            var next = current.Move(direction);
            if (!CoordToChar.ContainsKey(next))
            {
                return false;
            }
            current = next;
        }
        return true;
    }

    public string[] GetWholePaths(string buttons)
    {
        var paths = new string[buttons.Length];

        paths[0] = GetPath('A', buttons[0]);
        for (var i = 0; i < buttons.Length - 1; i++)
        {
            paths[i+1] = GetPath(buttons[i], buttons[i + 1]);
        }
        return paths;
    }

    public static Keypad NumericKeypad() => new(new()
    {
        { new Coordinate(0, 0), '7' },
        { new Coordinate(0, 1), '8' },
        { new Coordinate(0, 2), '9' },
        { new Coordinate(1, 0), '4' },
        { new Coordinate(1, 1), '5' },
        { new Coordinate(1, 2), '6' },
        { new Coordinate(2, 0), '1' },
        { new Coordinate(2, 1), '2' },
        { new Coordinate(2, 2), '3' },
        { new Coordinate(3, 1), '0' },
        { new Coordinate(3, 2), 'A' }
    });

    public static Keypad DirectionalKeypad() => new(new()
    {
        { new Coordinate(0, 1), '^' },
        { new Coordinate(0, 2), 'A' },
        { new Coordinate(1, 0), '<' },
        { new Coordinate(1, 1), 'v' },
        { new Coordinate(1, 2), '>' },
    });
}