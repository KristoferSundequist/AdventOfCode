public record Noop : IModule
{
    public required string Name { get; init; }

    public static Noop From(string name)
    {
        return new Noop { Name = name };
    }

    public List<Pulse> OnPulse(Pulse pulse, long iteration)
    {
        return [];
    }
}