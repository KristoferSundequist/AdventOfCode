class OrderingRules(List<OrderingRule> rules)
{
    public int Compare(int a, int b)
    {
        foreach (var rule in rules)
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
}