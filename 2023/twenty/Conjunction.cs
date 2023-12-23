public record Conjunction : IModule
{
    public required string Name { get; init; }
    public required Dictionary<string, PulseType> InputStates { get; set; }
    public required List<string> Destinations { get; init; }
    public Dictionary<string, long> FirstIteration { get; set; } = new();
    public Dictionary<string, long> PrevIteration { get; set; } = new();

    public static Conjunction From(string name, HashSet<string> inputs, List<string> destinations)
    {
        var inputStates = inputs.ToDictionary(input => input, _ => PulseType.Low);
        return new Conjunction { Name = name, InputStates = inputStates, Destinations = destinations };
    }

    public List<Pulse> OnPulse(Pulse pulse, long iteration)
    {
        InputStates[pulse.source] = pulse.type;
        if (Name == "lb")
        {
            foreach (var s in InputStates)
            {
                if (s.Value == PulseType.High)
                {
                    if (PrevIteration.TryGetValue(s.Key, out var prev))
                    {
                        if (prev != iteration)
                        {
                            Console.WriteLine($"{s.Key} cycle is {iteration - prev}");
                            PrevIteration[s.Key] = iteration;
                        }
                    }
                    else
                    {
                        FirstIteration[s.Key] = iteration;
                        PrevIteration[s.Key] = iteration;
                        Console.WriteLine($"{s.Key} is high first time on iteration {iteration}");
                    }
                }
            }
        }
        var sendPulseType = InputStates.All(kvp => kvp.Value == PulseType.High) ? PulseType.Low : PulseType.High;
        return Destinations.Select(d => new Pulse(Name, d, sendPulseType)).ToList();
    }
}