var line = File.ReadAllLines("./data.txt");
if (line.Length != 1) throw new Exception("Expected 1 line");
var stepStrs = line[0].Split(",");
var sum = 0;
foreach (var step in stepStrs)
{
    sum += RunHash(step);
}
Console.WriteLine($"Result 1: {sum}");


var boxes = Enumerable.Range(0, 256).Select(_ => new Box()).ToArray();
var steps = stepStrs.Select(str => Step.FromString(str));
foreach (var step in steps)
{
    var boxIndex = RunHash(step.Label);
    if (step.Operation == Operation.Add)
    {
        boxes[boxIndex].Upsert(new Lense(step.Label, step.FocalLength ?? throw new Exception("didnt expect null")));
    }
    else if (step.Operation == Operation.Remove)
    {
        boxes[boxIndex].Remove(step.Label);
    }
    else
    {
        throw new Exception($"Unkown operation {step.Operation}");
    }
    // Console.WriteLine($"After \"{step.RawString}\", label: {step.Label}");
    // PrintBoxes();
}

long focusingPowerSum = 0;
for (var i = 0; i < boxes.Length; i++)
{
    focusingPowerSum += (i + 1) * boxes[i].GetFocusingPower();
}
Console.WriteLine($"Result 2: {focusingPowerSum}");

int RunHash(string str)
{
    int currentValue = 0;
    foreach (var c in str)
    {
        var asAscii = (int)c;
        currentValue += asAscii;
        currentValue *= 17;
        currentValue = currentValue % 256;
    }
    return currentValue;
}

void PrintBoxes()
{
    for (var i = 0; i < boxes.Length; i++)
    {
        if (!boxes[i].IsEmpty())
        {
            Console.WriteLine($"Box {i}: {boxes[i].ToString()}");
        }
    }
}

public class Box
{
    private LinkedList<Lense> _lenses = new LinkedList<Lense>();

    public void Remove(string label)
    {
        if (_lenses.Nodes().FirstOrDefault(n => n.Value.label == label) is LinkedListNode<Lense> node)
        {
            _lenses.Remove(node);
        }
    }

    public bool IsEmpty() => _lenses.Count == 0;

    public override string ToString() => string.Join(" ", _lenses.Select(l => l.ToString()));

    public void Upsert(Lense newLense)
    {
        if (_lenses.Nodes().FirstOrDefault(n => n.Value.label == newLense.label) is LinkedListNode<Lense> node)
        {
            node.Value = newLense;
        }
        else
        {
            _lenses.AddLast(newLense);
        }
    }

    public long GetFocusingPower()
    {
        var sum = 0;
        var slot = 1;
        foreach (var node in _lenses)
        {
            sum += slot * node.focalLength;
            slot++;
        }
        return sum;
    }

}

public record Lense(string label, int focalLength)
{
    public override string ToString() => $"[{label} {focalLength}]";
}

public record Step
{
    public required string RawString { get; set; }
    public required string Label { get; set; }
    public required Operation Operation { get; set; }
    public int? FocalLength { get; set; } = null;

    public static Step FromString(string str)
    {
        if (str.Split("=") is [string label, string focalLengthStr])
        {
            return new Step { RawString = str, Label = label, Operation = Operation.Add, FocalLength = int.Parse(focalLengthStr) };
        }

        if (str.EndsWith("-"))
        {
            return new Step { RawString = str, Label = str[..^1], Operation = Operation.Remove, FocalLength = null };
        }

        throw new ArgumentException("Couldnt parse step");
    }
}
public enum Operation
{
    Add,
    Remove
}