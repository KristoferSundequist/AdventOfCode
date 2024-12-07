public record Equation(long TestValue, List<long> Numbers)
{
    public static Equation Parse(string line)
    {
        var parts = line.Split(":");
        var testValue = long.Parse(parts[0].Trim());
        var numbers = parts[1].Split(" ").Where(x => x != "").Select(long.Parse).ToList();
        return new Equation(testValue, numbers);
    }

    public override string ToString()
    {
        return $"{TestValue}: {string.Join(" ", Numbers)}";
    }

    public bool IsMatch(bool shouldConcat)
    {
        var sums = new List<long> { Numbers[0] };
        for (var i = 1; i < Numbers.Count; i++)
        {
            var newSums = new List<long>();
            foreach (var sum in sums)
            {
                newSums.Add(sum + Numbers[i]);
                newSums.Add(sum * Numbers[i]);
                if (shouldConcat)
                {
                    newSums.Add(long.Parse($"{sum}{Numbers[i]}"));
                }
            }
            sums = newSums.Where(x => x <= TestValue).ToList();
        }
        return sums.Contains(TestValue);
    }
}