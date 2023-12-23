public record FlipFlop : IModule
{
    public required string Name { get; init; }
    public required bool On { get; set; }
    public required List<string> Destinations { get; init; }

    public static FlipFlop From(string name, List<string> destinations)
    {
        return new FlipFlop { Name = name, On = false, Destinations = destinations };
    }

    public List<Pulse> OnPulse(Pulse pulse, long iteration)
    {
        if (pulse.type == PulseType.High)
        {
            return [];
        }
        On = !On;
        var sendPulseType = On ? PulseType.High : PulseType.Low;
        return Destinations.Select(d => new Pulse(Name, d, sendPulseType)).ToList();
    }
}