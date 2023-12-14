using System.Text;

var data = File.ReadLines("./data.txt").ToArray();
var schematic = new Schematic(data);

var result1 = schematic.GetAllSymbolAdjacentNumbersSum();
Console.WriteLine($"Result 1: {result1}");

var result2 = schematic.GetTwoPartGearSum();
Console.WriteLine($"Result 2: {result2}");

public class Schematic
{
    private Dictionary<Coordinate, char> SymbolMap = new Dictionary<Coordinate, char> { };
    private Dictionary<Coordinate, Number> NumberMap = new Dictionary<Coordinate, Number> { };

    public Schematic(string[] lines)
    {
        PopulateSymbolMap(lines);
        PopulateNumberMap(lines);
    }

    public long GetTwoPartGearSum()
    {
        long sum = 0;
        foreach (var kvp in SymbolMap.Where(kvp => kvp.Value == '*'))
        {
            var adjacentNumbers = GetAdjacentNumbers(kvp.Key);
            if (adjacentNumbers.Count == 2)
            {
                sum += adjacentNumbers.Aggregate((long)1, (product, v) => product * v.value);
            }
        }
        return sum;
    }

    public long GetAllSymbolAdjacentNumbersSum()
    {
        var symbolAdjacentNumbers = new HashSet<Number> { };
        foreach (var kvp in SymbolMap)
        {
            var adjacentNumbers = GetAdjacentNumbers(kvp.Key);
            foreach (var n in adjacentNumbers)
            {
                symbolAdjacentNumbers.Add(n);
            }
        }
        return symbolAdjacentNumbers.Sum(number => number.value);
    }

    private HashSet<Number> GetAdjacentNumbers(Coordinate coord)
    {
        var adjacentNumbers = new HashSet<Number> { };
        foreach (var adjacentCoord in coord.AdjacentCoordinates())
        {
            if (NumberMap.TryGetValue(adjacentCoord, out var number))
            {
                adjacentNumbers.Add(number);
            }
        }
        return adjacentNumbers;
    }

    private void PopulateSymbolMap(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                var c = line[x];
                if (!char.IsDigit(c) && c != '.')
                {
                    SymbolMap.Add(new Coordinate(y, x), c);
                }
            }
        }
    }

    private void PopulateNumberMap(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            var numberBuilder = new StringBuilder();
            for (var x = 0; x < line.Length; x++)
            {
                var c = line[x];
                if (char.IsDigit(c))
                {
                    numberBuilder.Append(c);
                    if (x == line.Length - 1)
                    {
                        var numberStr = numberBuilder.ToString();
                        AddNumber(y, x - numberStr.Length - 1, numberStr);
                        numberBuilder.Clear();
                    }
                }
                else if (numberBuilder.Length > 0)
                {
                    var numberStr = numberBuilder.ToString();
                    AddNumber(y, x - numberStr.Length, numberStr);
                    numberBuilder.Clear();
                }

            }
        }
    }

    private void AddNumber(int y, int x, string numberStr)
    {
        var guid = Guid.NewGuid();
        var number = int.Parse(numberStr);
        for (var i = 0; i < numberStr.Length; i++)
        {
            NumberMap.Add(new Coordinate(y, x + i), new Number(guid, number));
        }

    }

    private record Number(Guid id, int value);
}

public record Coordinate(int y, int x)
{
    public HashSet<Coordinate> AdjacentCoordinates()
    {
        return new()
        {
            new Coordinate(y - 1, x - 1),
            new Coordinate(y - 1, x),
            new Coordinate(y - 1, x + 1),
            new Coordinate(y, x - 1),
            new Coordinate(y, x + 1),
            new Coordinate(y + 1, x - 1),
            new Coordinate(y + 1, x),
            new Coordinate(y + 1, x + 1),
        };
    }
}