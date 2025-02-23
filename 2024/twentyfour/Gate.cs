public record Gate
{
    public string Input1 { get; init; }
    public string Input2 { get; init; }
    public string Output { get; init; }
    public string Operation { get; init; }

    public Gate(string input)
    {
        var parts = input.Split(" ");
        Input1 = parts[0];
        Operation = parts[1];
        Input2 = parts[2];
        Output = parts[4];
    }

    public bool Activate(bool input1, bool input2)
    {
        return Operation switch
        {
            "AND" => input1 && input2,
            "OR" => input1 || input2,
            "XOR" => input1 ^ input2,
            _ => throw new InvalidOperationException($"Unknown operation: {Operation}")
        };
    }

    public override string ToString()
    {
        return $"{Input1} {Operation} {Input2} -> {Output}";
    }
}