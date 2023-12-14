public class Almenac
{
    private List<TranslationMap> TranslationMaps = new();
    private List<long> Seeds = new();
    private List<SeedRange> SeedRanges = new();

    public Almenac(string rawInput)
    {
        var sections = rawInput.Split(
            new string[] { Environment.NewLine + Environment.NewLine },
            StringSplitOptions.RemoveEmptyEntries
        );

        Seeds = sections[0][6..].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
        for (var i = 0; i < Seeds.Count; i += 2)
        {
            SeedRanges.Add(new SeedRange(Seeds[i], Seeds[i + 1]));
        }
        TranslationMaps = sections[1..].Select(section => new TranslationMap(section)).ToList();
    }

    public long GetMinInNormalSeeds()
    {
        return Seeds.Select(TranslateSeed).Min();
    }

    public long GetMinInSeedRanges()
    {
        return SeedRanges.AsParallel().Select(MinInSeedRange).Min();
    }

    private long MinInSeedRange(SeedRange seedRange)
    {
        Console.WriteLine($"Starting seedrange with length {seedRange.Length}");
        var currentMin = long.MaxValue;
        for (var i = 0; i < seedRange.Length; i++)
        {
            currentMin = long.Min(currentMin, TranslateSeed(seedRange.Start + i));
        }
        return currentMin;
    }

    private long TranslateSeed(long seed)
    {
        var currentNumber = seed;
        foreach (var translationMap in TranslationMaps)
        {
            currentNumber = translationMap.SourceToDestination(currentNumber);
        }
        return currentNumber;
    }
}

public record SeedRange(long Start, long Length);

public class TranslationMap
{
    private List<Range> Ranges = new();

    public TranslationMap(string rawString)
    {
        var lines = rawString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Ranges = lines.Skip(1).Select(str => new Range(str)).ToList();
    }

    public long SourceToDestination(long number)
    {
        foreach (var range in Ranges)
        {
            if (range.IsInSourceRange(number))
            {
                return range.SourceToDestination(number);
            }
        }
        return number;
    }

}

public class Range
{
    private long SourceRangeStart;
    private long DestinationRangeStart;
    private long RangeLength;

    public Range(string rawString)
    {
        if (rawString.Split(" ") is [string destinationRangeStartStr, string sourceRangeStartStr, string rangeLengthStr])
        {
            DestinationRangeStart = long.Parse(destinationRangeStartStr);
            SourceRangeStart = long.Parse(sourceRangeStartStr);
            RangeLength = long.Parse(rangeLengthStr);
        }
        else
        {
            throw new Exception("range parse error");
        }
    }

    public bool IsInSourceRange(long number) => number >= SourceRangeStart && number < SourceRangeStart + RangeLength;
    public long SourceToDestination(long number) => DestinationRangeStart + (number - SourceRangeStart);

}