var freshRanges = File.ReadAllLines("freshranges.txt")
    .Select(line => line.Split('-'))
    .Select(parts => new Range(long.Parse(parts[0]), long.Parse(parts[1])))
    .ToList();

var ids = File.ReadAllLines("ids.txt")
    .Select(line => long.Parse(line))
    .ToList();

var part1 = ids.Count(id => freshRanges.Any(range => range.Contains(id)));
Console.WriteLine($"Part 1: {part1}");


var mergedRanges = MergeRanges(freshRanges);
var part2 = mergedRanges.Sum(r => r.GetSize());
Console.WriteLine($"Part 2: {part2}");

List<Range> MergeRanges(List<Range> ranges)
{
    while (true)
    {
        var newRanges = MergeRangesOnce(ranges);
        if (newRanges.Count == ranges.Count)
        {
            return newRanges;
        }
        ranges = newRanges;
    }
}

List<Range> MergeRangesOnce(List<Range> ranges)
{
    List<Range> newRanges = [];
    HashSet<int> mergedIndices = new();

    for (int i = 0; i < ranges.Count; i++)
    {
        if (mergedIndices.Contains(i))
        {
            continue;
        }
        var mergedRange = ranges[i];
        for (var j = i + 1; j < ranges.Count; j++)
        {
            if (mergedIndices.Contains(j))
            {
                continue;
            }
            if (mergedRange.Overlaps(ranges[j]))
            {
                mergedRange = mergedRange.Merge(ranges[j]);
                mergedIndices.Add(i);
                mergedIndices.Add(j);
            }
        }
        newRanges.Add(mergedRange);
    }
    return newRanges;
}

record Range(long Start, long End)
{
    public bool Contains(long id) => id >= Start && id <= End;

    public Range Merge(Range other)
    {
        var mergedStart = Math.Min(Start, other.Start);
        var mergedEnd = Math.Max(End, other.End);
        return new Range(mergedStart, mergedEnd);
    }

    public bool Overlaps(Range other) => !(other.End < Start || other.Start > End);

    public long GetSize() => End - Start + 1;
}