record Update()
{
    public required List<int> Pages { get; init; }

    public static Update FromString(string update)
    {
        return new Update
        {
            Pages = update.Split(',').Select(int.Parse).ToList()
        };
    }

    public override string ToString()
    {
        return string.Join(", ", Pages);
    }

    public int GetMiddleNumber()
    {
        var middleNumber = Pages[Pages.Count / 2];
        return middleNumber;
    }

    public bool IsCorrect(OrderingRules rules)
    {
        for (var i = 0; i < Pages.Count - 1; i++)
        {
            var disallowedNextPages = rules.GetDisallowedNextPages(Pages[i]);
            if (disallowedNextPages == null)
            {
                continue;
            }
            for (var j = i + 1; j < Pages.Count; j++)
            {
                if (disallowedNextPages.Contains(Pages[j]))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void SortPages(Comparison<int> comparison)
    {
        Pages.Sort(comparison);
    }
}