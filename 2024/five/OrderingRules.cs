using System.Security;

class OrderingRules
{
    private Dictionary<int, HashSet<int>> DisallowedNextPagesByFirstPage;
    private List<OrderingRule> _rules;

    public OrderingRules(List<OrderingRule> rules, HashSet<int> allPages)
    {
        DisallowedNextPagesByFirstPage = allPages
            .ToDictionary(p => p, p => CalculateDisallowedNextPages(rules, p));
        _rules = rules;
    }

    public HashSet<int>? GetDisallowedNextPages(int page)
    {
        return DisallowedNextPagesByFirstPage.TryGetValue(page, out var disallowedNextPages)
            ? disallowedNextPages
            : null;
    }

    public int Compare(int a, int b)
    {
        foreach(var rule in _rules)
        {
            if (rule.Page1 == a && rule.Page2 == b)
            {
                return 1;
            }
            if (rule.Page2 == a && rule.Page1 == b)
            {
                return -1;
            }
        }
        return 0;
    }

    private HashSet<int> CalculateDisallowedNextPages(List<OrderingRule> rules, int page)
    {
        return rules.Where(r => r.Page2 == page)
            .Select(r => r.Page1)
            .ToHashSet();
    }
}