public class Device
{
    public Dictionary<string, bool> Wires { get; set; } = new();
    public Dictionary<string, bool> InitialWires { get; } = new();
    public Dictionary<string, Gate> Gates { get; } = new();

    public Device(string[] initial, string[] gates)
    {
        foreach (var line in initial)
        {
            var parts = line.Split(": ");
            Wires[parts[0]] = parts[1] == "1";
            InitialWires[parts[0]] = parts[1] == "1";
        }

        foreach (var line in gates)
        {
            var gate = new Gate(line);
            Gates[gate.Output] = gate;
        }
    }

    public long GetFirstInput()
    {
        var binary = InitialWires.Where(w => w.Key.StartsWith("x")).Select(w => w.Value ? "1" : "0").OrderDescending();
        var asStr = string.Join("", binary);
        var decimalValue = Convert.ToInt64(asStr, 2);
        return decimalValue;
    }

    public long GetSecondInput()
    {
        var binary = InitialWires.Where(w => w.Key.StartsWith("y")).Select(w => w.Value ? "1" : "0").OrderDescending();
        var asStr = string.Join("", binary);
        var decimalValue = Convert.ToInt64(asStr, 2);
        return decimalValue;
    }

    public void Reset()
    {
        Wires = new Dictionary<string, bool>(InitialWires);
    }

    public void Run()
    {
        var initialGates = Gates.Values.Where(g => Wires.ContainsKey(g.Input1) && Wires.ContainsKey(g.Input2)).ToList();

        var outputs = InitialWires.Keys.ToHashSet();
        while (outputs.Count > 0)
        {
            outputs = Advance(outputs);
        }
    }

    public long GetOutput()
    {
        var outputWires = Wires.Where(w => w.Key.StartsWith("z")).ToList();
        var orderedWires = outputWires.OrderByDescending(w => w.Key).Select(w => w.Value ? "1" : "0");
        var asStr = string.Join("", orderedWires);
        var decimalValue = Convert.ToInt64(asStr, 2);
        return decimalValue;
    }

    private HashSet<string> Advance(HashSet<string> prevOutputs)
    {
        Console.WriteLine("---------------------------------------------");
        var gatesToActivate = Gates.Values
            .Where(g => prevOutputs.Contains(g.Input1) || prevOutputs.Contains(g.Input2)) // Only activate if we just got inputs (no loops)
            .Where(g => Wires.ContainsKey(g.Input1) && Wires.ContainsKey(g.Input2)); // Only activate if we have both inputs


        var newOutputs = new HashSet<string>();
        foreach (var gate in gatesToActivate.OrderBy(g => g.Operation))
        {
            Console.WriteLine(gate);
            var input1 = Wires[gate.Input1];
            var input2 = Wires[gate.Input2];
            var output = gate.Activate(input1, input2);
            Wires[gate.Output] = output;
            newOutputs.Add(gate.Output);
        }

        return newOutputs;
    }

    public void Visualize()
    {
        VisualizeGates();
    }

    private void VisualizeGates()
    {
        var found = new HashSet<string>();
        var gates = Gates.Values.Where(g => InitialWires.ContainsKey(g.Input1) || InitialWires.ContainsKey(g.Input2)).ToDictionary(g => g.Output);
        while (gates.Count > 0)
        {
            Console.WriteLine("--------------------");
            foreach (var gate in gates)
            {
                Console.WriteLine(gate.Value);
                found.Add(gate.Key);
            }
            gates = Gates.Values.Where(g => gates.ContainsKey(g.Input1) || gates.ContainsKey(g.Input2)).Where(g => !found.Contains(g.Output)).ToDictionary(g => g.Output);
        }
    }
}