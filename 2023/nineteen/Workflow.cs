public record Workflow
{
    public required string Name { get; init; }
    public required List<Rule> Rules { get; init; }
    public required string FailDestiniation { get; set; }

    public static Workflow FromString(string str)
    {
        var firstSplit = str.Split('{');
        var name = firstSplit[0];
        var rulesStr = firstSplit[1][..^1];
        var rulesStrs = rulesStr.Split(",");
        var rules = rulesStrs[..^1].Select(Rule.FromString).ToList();
        var failDestination = rulesStrs[^1];
        return new Workflow { Name = name, Rules = rules, FailDestiniation = failDestination };
    }

    public string Next(PartRating part)
    {
        foreach (var rule in Rules)
        {
            var result = rule.Check(part);
            if (result.isSuccess)
            {
                return rule.DoOnSuccess;
            }
        }
        return FailDestiniation;
    }

    public List<RangeResult> NextRanges(Ranges ranges)
    {
        var newRanges = new List<RangeResult>();
        var currentRanges = ranges;
        foreach (var rule in Rules)
        {
            var result = rule.Split(currentRanges);
            newRanges.Add(new RangeResult(result.successRanges, result.newWorkflowName));
            currentRanges = result.failureRanges;
        }
        newRanges.Add(new RangeResult(currentRanges, FailDestiniation));
        return newRanges.ToList();
    }
}

public record RangeResult(Ranges ranges, string newDestination);