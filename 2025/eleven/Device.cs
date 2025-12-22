public class Device
{
    public string Name;
    public HashSet<string> Outputs;

    public Device(string name, HashSet<string> outputs)
    {
        Name = name;
        Outputs = outputs;
    }

    public static Device FromString(string line)
    {
        var parts = line.Split(":");
        return new Device(parts[0], parts[1].Trim().Split(" ").ToHashSet());
    }

    public override string ToString()
    {
        return $"{Name}: {string.Join(" ", Outputs)}";
    }
}