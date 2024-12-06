record OrderingRule(int Page1, int Page2)
{
    public static OrderingRule FromString(string rule)
    {
        var pages = rule.Split('|');
        return new OrderingRule(int.Parse(pages[0]), int.Parse(pages[1]));
    }
}