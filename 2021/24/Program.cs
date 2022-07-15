var lines = System.IO.File.ReadAllLines("data.txt").ToList();

// for (long i = 1; i < 10; i++)
// {
//     var input = new List<long> { };
//     var zero = false;
//     for (var j = 0; j < 15; j++)
//     {
//         // if (c == '0')
//         // {
//         //     zero = true;
//         // }
//         input.Add(long.Parse(i.ToString()));
//     }
//     if (zero)
//     {
//         continue;
//     }
//     var input = Enumerable.Range(0, 14).Select(_ => i).ToList();
//     Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(input));
//     if (IsValid(lines, input))
//     {
//         Console.WriteLine($"FOUND IT: {i}");
//         break;
//     }
// }

// var curMin = long.MaxValue;
// for (long i = 1; i < 10; i++)
// {
//     for (long j = 1; j < 10; j++)
//     {
//         for (long z = 1; z < 10; z++)
//         {
//             for (long i4 = 1; i4 < 10; i4++)
//             {
//                 for (long i5 = 1; i5 < 10; i5++)
//                 {
//                     for (long i6 = 1; i6 < 10; i6++)
//                     {
//                         for (long i7 = 1; i7 < 10; i7++)
//                         {
//                             var v = IsValid(lines.GetRange(0, 19 * 7), new List<long> { i, j, z, i4, i5, i6, i7, 2, 3, 3, 1, 1, 1, 3 });
//                             if (v < curMin)
//                             {
//                                 Console.WriteLine($"{i}, {j}, {z}, {i4}, {i5}, {i6}, {i7}: {v}");
//                                 curMin = v;
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//     }
// }
//var inp = new List<long> { 3, 8, 1, 1, 8, 1, 5, 4, 3, 5, 4, 3, 1, 9, 7 };
var inp = new List<long> { 3, 9, 9, 9, 9, 6, 9, 8, 7, 9, 9, 4, 2, 9 };
var v = IsValid(lines, inp);
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(inp) + ": " + v);

long IsValid(List<string> instructions, List<long> inputs)
{
    var n_inputs = 0;

    long w = 0;
    long x = 0;
    long y = 0;
    long z = 0;

    void SetVariableTo(string var, long value)
    {
        if (var == "w")
        {
            w = value;
        }
        else if (var == "x")
        {
            x = value;
        }
        else if (var == "y")
        {
            y = value;
        }
        else if (var == "z")
        {
            z = value;
        }
        else
        {
            throw new Exception($"Unknown var {var}");
        }
    }

    long GetVariableValue(string var) =>
        var switch
        {
            "w" => w,
            "x" => x,
            "y" => y,
            "z" => z,
            _ => throw new Exception($"Unkown variable {var}")
        };

    long GetValue(string variableOrValue) =>
        long.TryParse(variableOrValue, out long value) ? value : GetVariableValue(variableOrValue);

    foreach (var instruction in instructions)
    {
        var split = instruction.Split(" ");
        switch (split[0])
        {
            case "inp":
                HandleInput(split[1]);
                break;
            case "add":
                HandleAdd(split[1], split[2]);
                break;
            case "mul":
                HandleMul(split[1], split[2]);
                break;
            case "div":
                HandleDiv(split[1], split[2]);
                break;
            case "mod":
                HandleMod(split[1], split[2]);
                break;
            case "eql":
                HandleEql(split[1], split[2]);
                break;
            default:
                throw new Exception($"Unexpected instruction: {split[0]}");
        };
    }

    // Console.WriteLine($"w: {w} x: {x} y: {y} z: {z}");
    return z;

    void HandleInput(string variable)
    {
        SetVariableTo(variable, inputs[n_inputs]);
        n_inputs++;
        Console.WriteLine($"z: {z}");
    }

    void HandleAdd(string variable, string otherValue)
    {
        var newValue = GetVariableValue(variable) + GetValue(otherValue);
        SetVariableTo(variable, newValue);
    }

    void HandleMul(string variable, string otherValue)
    {
        var newValue = GetVariableValue(variable) * GetValue(otherValue);
        SetVariableTo(variable, newValue);
    }

    void HandleDiv(string variable, string otherValue)
    {
        var otherValueValue = GetValue(otherValue);
        if (otherValueValue == 0)
        {
            throw new Exception("div with zero");
        }
        var newValue = GetVariableValue(variable) / otherValueValue;
        SetVariableTo(variable, newValue);
    }

    void HandleMod(string variable, string otherValue)
    {
        var varValue = GetVariableValue(variable);
        var otherValueValue = GetValue(otherValue);
        if (varValue < 0 || otherValueValue <= 0)
        {
            throw new Exception("mod with 0");
        }

        var newValue = varValue % otherValueValue;
        SetVariableTo(variable, newValue);
    }

    void HandleEql(string variable, string otherValue)
    {
        var newValue = GetVariableValue(variable) == GetValue(otherValue);
        SetVariableTo(variable, newValue ? 1 : 0);
    }
}
