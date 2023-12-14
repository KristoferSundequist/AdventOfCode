var data = File.ReadAllLines("data.txt");

var result = data.Select(GetNumber1).Sum();
Console.WriteLine($"Part1: {result}");

var result2 = data.Select(GetNumber2).Sum();
Console.WriteLine($"Part2: {result2}");

int GetNumber1(string line)
{
    var startChar = "";
    var endChar = "";
    for (var i = 0; i < line.Length; i++)
    {
        if (startChar == "" && Char.IsDigit(line[i]))
        {
            startChar = line[i].ToString();
        }
        var end = line.Length - i - 1;
        if (endChar == "" && Char.IsDigit(line[end]))
        {
            endChar = line[end].ToString();
        }
    }

    return int.Parse($"{startChar}{endChar}");
}

bool PrefixIsStringNumber(string str, string numberStr)
{
    if (numberStr.Length > str.Length)
    {
        return false;
    }
    return str[..numberStr.Length] == numberStr;
}

int? GetStringNumber(string str)
{
    string[] numberStrings = [
        "one","two","three","four","five","six","seven","eight", "nine"
    ];
    for (var i = 0; i < numberStrings.Length; i++)
    {
        if (PrefixIsStringNumber(str, numberStrings[i]))
        {
            return i + 1;
        }
    }
    return null;
}

int GetNumber2(string line)
{
    var startChar = "";
    var endChar = "";
    for (var i = 0; i < line.Length; i++)
    {
        if (startChar == "")
        {
            var maybeStringNumber = GetStringNumber(line[i..]);
            if (maybeStringNumber is int siffra)
            {
                startChar = $"{siffra}";
            }
            else if (Char.IsDigit(line[i]))
            {
                startChar = line[i].ToString();
            }
        }
        var endIndex = line.Length - i - 1;

        if (endChar == "")
        {
            var maybeStringNumber = GetStringNumber(line[endIndex..]);
            if (maybeStringNumber is int siffra)
            {
                endChar = $"{siffra}";
            }
            else if (Char.IsDigit(line[endIndex]))
            {
                endChar = line[endIndex].ToString();
            }
        }
    }

    return int.Parse($"{startChar}{endChar}");
}