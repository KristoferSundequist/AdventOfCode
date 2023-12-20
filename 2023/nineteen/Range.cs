public record Ranges(Range xs, Range ms, Range aas, Range ss)
{
    public Ranges With(char attribute, Range newRange) =>
        attribute switch
        {
            'x' => this with { xs = newRange },
            'a' => this with { aas = newRange },
            'm' => this with { ms = newRange },
            's' => this with { ss = newRange },
            _ => throw new Exception("With selection error")
        };

    public long Product() => xs.Size() * ms.Size() * aas.Size() * ss.Size();
    public (Ranges, Ranges) Split(char attribute, int splitAt)
    {
        var rangeToSplit = attribute switch
        {
            'x' => xs,
            'a' => aas,
            'm' => ms,
            's' => ss,
            _ => throw new Exception("split Attirbute selection error")
        };
        var (range1, range2) = rangeToSplit.Split(splitAt);
        return (this.With(attribute, range1), this.With(attribute, range2));
    }

}

public record Range(int start, int end)
{
    public (Range, Range) Split(int splitAt)
    {
        return (new Range(start, splitAt), new Range(splitAt + 1, end));
    }

    public long Size()
    {
        if (end < start)
        {
            return 0;
        }
        return end - start + 1;
    }

    public override string ToString() => $"({start},{end})";
}