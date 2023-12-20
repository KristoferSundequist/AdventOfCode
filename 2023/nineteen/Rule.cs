public record Rule
{

    public required char EqualityOperation { get; init; }
    public char AttributeToCheck { get; init; }
    public int ContantToCheckAgainst { get; init; }
    public required string DoOnSuccess { get; init; }

    public static Rule FromString(string str)
    {
        if (str.Split(":") is [string check, string doOnSuccess])
        {
            if (check is [char attribute, char operation, .. string constant])
            {
                var eqOperation = operation switch
                {
                    '<' => '<',
                    '>' => '>',
                    _ => throw new Exception($"eq operation parse error unexpectex {operation}")
                };
                return new Rule { EqualityOperation = eqOperation, AttributeToCheck = attribute, ContantToCheckAgainst = int.Parse(constant), DoOnSuccess = doOnSuccess };
            }
            else
            {
                throw new Exception("rule parse error left parse");
            }
        }
        else
        {
            throw new Exception("rule parse error first split");
        }
    }

    public RuleResult Check(PartRating part)
    {
        var attributeValue = AttributeToCheck switch
        {
            'x' => part.x,
            'a' => part.a,
            'm' => part.m,
            's' => part.s,
            _ => throw new Exception("attirbyute selection error")
        };
        var isSuccess = EqualityOperation switch
        {
            '>' => attributeValue > ContantToCheckAgainst,
            '<' => attributeValue < ContantToCheckAgainst,
            _ => throw new Exception("Unknown eq operation when checking")
        };
        if (isSuccess)
        {
            return new RuleResult(isSuccess, DoOnSuccess);
        }
        else
        {
            return new RuleResult(false, null);
        }
    }

    public RuleRangesResult Split(Ranges ranges)
    {
        var splitAt = EqualityOperation == '>' ? ContantToCheckAgainst : ContantToCheckAgainst - 1;
        var (ranges1, ranges2) = ranges.Split(AttributeToCheck, splitAt);
        var (winRanges, loseRanges) = EqualityOperation == '>' ? (ranges2, ranges1) : (ranges1, ranges2);
        return new RuleRangesResult(winRanges, DoOnSuccess, loseRanges);

    }
}
public record RuleRangesResult(Ranges successRanges, string newWorkflowName, Ranges failureRanges);
public record RuleResult(bool isSuccess, string? newWorkflowName);