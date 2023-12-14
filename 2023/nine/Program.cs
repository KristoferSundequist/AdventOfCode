var lines = File.ReadAllLines("./data.txt");
var sequences = lines.Select(Sequence.FromString).ToArray();

var result = sequences.Select(seq => seq.Extrapolate()).Sum();
Console.WriteLine($"Result 1: {result}");

var result2 = sequences.Select(seq => seq.ExtrapolateBackwards()).Sum();
Console.WriteLine($"Result 2: {result2}");

public class Sequence
{
    private long[] Numbers = [];

    public static Sequence FromString(string str)
    {
        return new Sequence { Numbers = str.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray() };
    }

    public long Extrapolate()
    {
        if (Numbers.All(v => v == 0))
        {
            return 0;
        }
        return Numbers.Last() + GetDiffSequence().Extrapolate();
    }

    public long ExtrapolateBackwards()
    {
        if (Numbers.All(v => v == 0))
        {
            return 0;
        }
        return Numbers.First() - GetDiffSequence().ExtrapolateBackwards();
    }

    private Sequence GetDiffSequence()
    {
        long[] diffs = new long[Numbers.Length - 1];
        for (var i = 1; i < Numbers.Length; i++)
        {
            diffs[i - 1] = Numbers[i] - Numbers[i - 1];
        }
        return new Sequence { Numbers = diffs };
    }
}
