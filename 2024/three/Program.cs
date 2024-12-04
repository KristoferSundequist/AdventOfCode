var input = File.ReadAllText("data.txt");

Part1();
Part2();

void Part1()
{
    long sum = 0;
    for (var i = 0; i < input.Length; i++)
    {
        if (TryParseMultiplication(input[i..], out var result))
        {
            sum += result.Value.v1 * result.Value.v2;
        }
    }
    Console.WriteLine($"Part 1: {sum}");
}

void Part2()
{
    long sum = 0;
    bool isActive = true;
    for (var i = 0; i < input.Length; i++)
    {
        if (TryParseDo(input[i..]))
        {
            isActive = true;
        }
        if (TryParseDont(input[i..]))
        {
            isActive = false;
        }
        if (isActive && TryParseMultiplication(input[i..], out var result))
        {
            sum += result.Value.v1 * result.Value.v2;
        }
    }
    Console.WriteLine($"Part 2: {sum}");
}



bool TryParseMultiplication(string input, out ParseResult<Multiplication> result)
{
    if (input.Length > 4 && input.StartsWith("mul("))
    {
        var remaining = input[4..];
        if (TryParseNumber(remaining, out var number1Result))
        {
            remaining = number1Result.Remaining;
            if (remaining.StartsWith(","))
            {
                remaining = remaining[1..];
                if (TryParseNumber(remaining, out var number2Result))
                {
                    remaining = number2Result.Remaining;
                    if (remaining.StartsWith(")"))
                    {
                        result = new ParseResult<Multiplication>(new Multiplication(number1Result.Value, number2Result.Value), remaining[1..]);
                        return true;
                    }
                }
            }
        }
    }
    result = new ParseResult<Multiplication>(new Multiplication(0, 0), "");
    return false;
}

bool TryParseNumber(string input, out ParseResult<long> result)
{
    if (input.Length >= 3 && input[0..3].All(char.IsDigit))
    {
        var number = long.Parse(input[0..3]);
        var remaining = input[3..];
        result = new ParseResult<long>(number, remaining);
        return true;
    }
    if (input.Length >= 2 && input[0..2].All(char.IsDigit))
    {
        var number = long.Parse(input[0..2]);
        var remaining = input[2..];
        result = new ParseResult<long>(number, remaining);
        return true;
    }
    if (input.Length >= 1 && char.IsDigit(input[0]))
    {
        var number = long.Parse(input[0].ToString());
        var remaining = input[1..];
        result = new ParseResult<long>(number, remaining);
        return true;
    }
    result = new ParseResult<long>(0, "");
    return false;
}

bool TryParseDo(string input)
{
    if (input.Length >= 4 && input[..4] is "do()")
    {
        return true;
    }
    return false;
}

bool TryParseDont(string input)
{
    if (input.Length >= 7 && input[..7] is "don't()")
    {
        return true;
    }
    return false;
}

record ParseResult<T>(T Value, string Remaining);
record Multiplication(long v1, long v2);

