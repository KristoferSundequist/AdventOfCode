public class Secret
{
    private long _seed;
    private List<long> _prices;
    private List<long> _diffs;
    private Dictionary<string, long> priceByDiff = new();

    public Secret(long seed, int n)
    {
        _seed = seed;
        _prices = GetPrices(seed, n);
        _diffs = GetDiffs(_prices);
        PopulatePriceByDiff();
    }

    public void Visualize()
    {
        Console.WriteLine("Prices:");
        Console.WriteLine(string.Join(", ", _prices));
        Console.WriteLine("Diffs:");
        Console.WriteLine(string.Join(", ", _diffs));
        Console.WriteLine("Price by diff:");
        foreach (var kvp in priceByDiff)
        {
            Console.WriteLine($"{kvp.Key} -> {kvp.Value}");
        }
    }

    public HashSet<string> GetAllDiffKeys() => new(priceByDiff.Keys);
    public long GetPriceByDiff(string key) => priceByDiff.GetValueOrDefault(key, 0);

    private void PopulatePriceByDiff()
    {
        for (int i = 0; i < _diffs.Count - 3; i++)
        {
            var key = GetKey(_diffs[i], _diffs[i + 1], _diffs[i + 2], _diffs[i + 3]);

            if (!priceByDiff.ContainsKey(key))
            {
                priceByDiff[key] = _prices[i + 4];
            }
        }
    }

    private string GetKey(long a, long b, long c, long d) => $"{a},{b},{c},{d}";

    private long Next(long prev)
    {
        prev = Prune(Mix(prev, prev * 64));
        prev = Prune(Mix(prev, prev / 32));
        prev = Prune(Mix(prev, prev * 2048));
        return prev;
    }

    private long Mix(long a, long b) => a ^ b;
    private long Prune(long a) => a % 16777216;

    private List<long> GetDiffs(List<long> prices)
    {
        var diffs = new List<long>();
        for (int i = 1; i < prices.Count; i++)
        {
            diffs.Add(prices[i] - prices[i - 1]);
        }
        return diffs;
    }

    private long GetLastDigit(long n) => n % 10;

    private List<long> GetPrices(long start, int n)
    {
        var prices = new List<long> { GetLastDigit(start) };
        for (int i = 0; i < n - 1; i++)
        {
            start = Next(start);
            prices.Add(GetLastDigit(start));
        }
        return prices;
    }

    public long GetNthSecretNumber(int n)
    {
        var number = _seed;
        for (int i = 0; i < n; i++)
        {
            number = Next(number);
        }
        return number;
    }
}