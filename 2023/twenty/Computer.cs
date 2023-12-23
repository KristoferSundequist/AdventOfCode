public class Computer
{
    public readonly Dictionary<string, IModule> _modules;
    public readonly Dictionary<string, List<string>> _nameToDestinations;
    private long _lowPulses = 0;
    private long _highPulses = 0;
    private Queue<Pulse> _pulses = new();

    public Computer(string[] lines)
    {
        _nameToDestinations = GetNameToDestinations(lines);
        var nameToInputs = GetNameToInputs(_nameToDestinations);

        var modules = new Dictionary<string, IModule>();
        foreach (var line in lines)
        {
            var namePart = line.Split(" -> ")[0];
            if (namePart == "broadcaster")
            {
                continue;
            }
            var moduleType = namePart[0];
            var name = namePart[1..];
            if (moduleType == '%')
            {
                modules.Add(name, FlipFlop.From(name, _nameToDestinations[name]));
            }
            else if (moduleType == '&')
            {
                modules.Add(name, Conjunction.From(name, nameToInputs[name], _nameToDestinations[name]));
            }
            else
            {
                throw new Exception($"Unknown module type {moduleType}");
            }
        }
        foreach (var kvp in nameToInputs)
        {
            if (!modules.ContainsKey(kvp.Key))
            {
                modules.Add(kvp.Key, Noop.From(kvp.Key));
            }
        }
        _modules = modules;
    }

    public void ProcessPulses(long iteration, bool verbose)
    {
        while (_pulses.Count > 0)
        {
            var pulse = _pulses.Dequeue();
            if (pulse.destination == "rx" && pulse.type == PulseType.Low)
            {
                Console.WriteLine($"rx got low pulse on button press: {iteration}");
                throw new Exception("Program finished");
            }
            if (verbose)
            {
                Console.WriteLine(pulse);
            }
            if (pulse.type == PulseType.High)
            {
                _highPulses++;
            }
            else
            {
                _lowPulses++;
            }
            var module = _modules[pulse.destination];
            var newPulses = module.OnPulse(pulse, iteration);
            foreach (var newPulse in newPulses)
            {
                _pulses.Enqueue(newPulse);
            }
        }
    }

    public void PushButton(long iteration, bool verbose)
    {
        var startDestinations = _nameToDestinations["broadcaster"]!;
        _lowPulses++;
        foreach (var d in startDestinations)
        {
            _pulses.Enqueue(new Pulse("broadcaster", d, PulseType.Low));
        }
        ProcessPulses(iteration, verbose);
    }

    public long GetScore()
    {
        Console.WriteLine(_lowPulses);
        Console.WriteLine(_highPulses);
        return _lowPulses * _highPulses;
    }

    private static Dictionary<string, List<string>> GetNameToDestinations(string[] lines)
    {
        return lines.Select(line =>
        {
            var split = line.Split(" -> ");
            var namePart = split[0];
            var name = namePart == "broadcaster" ? namePart : namePart[1..];
            var destinations = split[1].Split(", ").ToList();
            return (name, destinations);
        }).ToDictionary(tuple => tuple.name, tuple => tuple.destinations);
    }

    private static Dictionary<string, HashSet<string>> GetNameToInputs(Dictionary<string, List<string>> nameToDestination)
    {
        var nameToInputs = new Dictionary<string, HashSet<string>>();
        foreach (var kvp in nameToDestination)
        {
            foreach (var destination in kvp.Value)
            {
                if (nameToInputs.TryGetValue(destination, out var current))
                {
                    current.Add(kvp.Key);
                }
                else
                {
                    nameToInputs.Add(destination, [kvp.Key]);
                }
            }
        }
        return nameToInputs;
    }

}